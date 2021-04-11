using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Parser.Nodes;

namespace Parser
{
    public class XamlParser
    {
        private readonly IErrorHandler _errorHandler;

        public XamlParser(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public XamlMainObjectNode Parse(string fileName)
        {
            if (fileName != null)
            {
                using StreamReader fileStream = new StreamReader(fileName);
                return Parse(fileStream);
            }

            return null;
        }

        public XamlMainObjectNode Parse(StreamReader fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            Stack<XamlObjectNode> objStack = new Stack<XamlObjectNode>();
            XamlObjectNode currentNode = null;
            XamlMainObjectNode mainObject = null;

            SymbolScanner scanner = new SymbolScanner(fileStream);

            int lineNumberStart = 0;
            int linePositionStart = 0;
            //long newLineStreamPosition = fileStream.BaseStream.Position;
            string ident;

            //bool tagBracketClosed = false;
            while (!fileStream.EndOfStream)
            {
                scanner.SkipSpaces();
                char ch = scanner.GetSymbol();
                if (ch == '\0')
                {
                    break;
                }

                if (ch == SymbolScanner.SymbolStartTag)
                {
                    //tagBracketClosed = false;
                    lineNumberStart = scanner.LineNumber;
                    //open tag symbol must be start not the next symbol
                    linePositionStart = scanner.CharPosition - 1;
                    char chNext = scanner.NextSymbol;
                    if (chNext == SymbolScanner.SymbolCloseTag)
                    {
                        scanner.SkipSymbol();
                        ident = scanner.GetName();
                        if (DoEndTag(ident, scanner))
                        {
                            CloseTag(ident, scanner, objStack);
                        }
                    }
                    else if (chNext == SymbolScanner.SymbolStartComment)
                    {
                        string comment = scanner.ReadComment();
                        Trace.WriteLine($"{lineNumberStart + 1}:{linePositionStart + 1} Comment:{comment}");
                        XamlCommentNode commentNode = new XamlCommentNode
                                                          {
                                                              LineNumberStart = lineNumberStart + 1,
                                                              LinePositionStart = linePositionStart + 1,
                                                              Comment = comment
                                                          };
                        commentNode.LineNumberEnd = scanner.LineNumber + 1;
                        //end position is outside comment tag
                        commentNode.LinePositionEnd = scanner.CharPosition;
                        objStack.Push(commentNode);
                    }
                    else
                    {
                        char breakSymbol;
                        ident = scanner.GetName();
                        breakSymbol = scanner.NextSymbol;
                        //ident = GetIdent(fileStream, out breakSymbol);
                        Trace.WriteLine($"{lineNumberStart + 1}:{linePositionStart + 1} Start Tag:{ident}");
                        if (mainObject == null)
                        {
                            mainObject = new XamlMainObjectNode { LineNumberStart = lineNumberStart + 1, LinePositionStart = linePositionStart + 1 };
                            mainObject.Name = ident;
                            currentNode = mainObject;
                            //root.MainObject = mainObject;
                            objStack.Push(mainObject);
                        }
                        else
                        {
                            currentNode = new XamlObjectNode { LineNumberStart = lineNumberStart + 1, LinePositionStart = linePositionStart + 1 };
                            currentNode.Name = ident;
                            objStack.Push(currentNode);
                        }

                        if (breakSymbol != SymbolScanner.SymbolEndTag)
                        {
                            scanner.SkipSpaces();
                            breakSymbol = scanner.NextSymbol;
                        }

                        if (breakSymbol == SymbolScanner.SymbolCloseTag)
                        {
                            //end of tag
                            if (DoEndTag(ident, scanner))
                            {
                                CloseTag(ident, scanner, objStack);
                            }
                        }
                        else if (breakSymbol != SymbolScanner.SymbolEndTag)
                        {
                            XamlObjectNode objectNode = null;
                            if (objStack.Count > 0)
                            {
                                objectNode = objStack.Peek();
                            }

                            bool isClosed = ReadAttributes(objectNode, scanner, breakSymbol);
                            if (isClosed)
                            {
                                //tagBracketClosed = true;
                                if (objectNode != null)
                                {
                                    objectNode.SetState(XamlNodeBase.EState.EndTagPresent);
                                    //objectNode.IsTagBracketClosed = true;
                                }

                                Trace.WriteLine("Tag end symbol for:" + ident);
                            }

                            if (scanner.NextSymbol == SymbolScanner.SymbolCloseTag)
                            {
                                //end of tag
                                if (DoEndTag(ident, scanner))
                                {
                                    CloseTag(ident, scanner, objStack);
                                }
                            }
                        }
                        else
                        {
                            // skip '>'
                            scanner.SkipSymbol();

                            XamlObjectNode objectNode = objStack.Peek();
                            objectNode.SetState(XamlNodeBase.EState.EndTagPresent);
                            Trace.WriteLine("Tag end symbol for:" + ident);
                        }

                        //chNext = scanner.NextSymbol;
                    }
                }
                else
                {
                    if (objStack.Count > 0)
                    {
                        XamlObjectNode objectNode = objStack.Peek();
                        if (objectNode.IsState(XamlNodeBase.EState.EndTagPresent))
                        {
                            lineNumberStart = scanner.LineNumber;
                            //text symbol must be current not the next symbol
                            linePositionStart = scanner.CharPosition - 1;

                            string text = scanner.ReadText(ch);
                            Trace.WriteLine("Text node:" + text);
                            XamlTextNode textNode = new XamlTextNode(text)
                                                        {
                                                            LineNumberStart = lineNumberStart + 1, LinePositionStart = linePositionStart + 1
                                                        };
                            textNode.LineNumberEnd = scanner.LastTextLineNumberEnd + 1;
                            // it must be the next symbol
                            textNode.LinePositionEnd = scanner.LastTextCharPositionEnd;
                            objectNode.SetState(XamlNodeBase.EState.TextNodePresent);
                            objectNode.AddChild(textNode);
                        }
                        else
                        {
                            //Trace.Write(ch);
                            _errorHandler?.Error($"Not expected symbol :'{ch}'", scanner);
                        }
                    }
                    else
                    {
                        //Trace.Write(ch);
                        _errorHandler?.Error($"Not expected symbol, no tag found :'{ch}'", scanner);
                    }
                }

                char chNext1 = scanner.CheckEndOfLine(scanner.NextSymbol);
            }

            if (objStack.Count > 0)
            {
                //ignore exception by non closed main state. Temporary
                if (mainObject != null && objStack.Count == 1)
                {
                    if (!mainObject.IsState(XamlNodeBase.EState.Closed))
                    {
                        return mainObject;
                    }
                }
                _errorHandler?.Error($"unused {objStack.Count} nodes", scanner);
            }

            return mainObject;
        }

        public XamlMainObjectNode ParseFromString(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            using MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(text));
            using StreamReader fileStream = new StreamReader(ms, true);
            return Parse(fileStream);
        }

        private static void CloseTag(string name, SymbolScanner scanner, Stack<XamlObjectNode> objStack)
        {
            XamlObjectNode objectNode = null;

            List<XamlObjectNode> children = new List<XamlObjectNode>();

            while (objStack.Count > 0)
            {
                objectNode = objStack.Peek();
                if (objectNode.Name == name)
                {
                    objectNode.LineNumberEnd = scanner.LineNumber + 1;
                    // it must be point to the next symbol so '-1'
                    objectNode.LinePositionEnd = scanner.CharPosition;
                    foreach (XamlObjectNode node in children)
                    {
                        objectNode.AddChild(node);
                    }

                    objectNode.SetState(XamlNodeBase.EState.Closed);

                    //remove found node from stack
                    objStack.Pop();
                    break;
                }

                children.Add(objectNode);
                objectNode = objStack.Pop();
            }
        }

        private bool DoEndTag(string ident, SymbolScanner scanner)
        {
            bool ret = false;
            Trace.WriteLine("End Tag:" + ident);
            scanner.SkipSpaces();
            if (scanner.NextSymbol == SymbolScanner.SymbolCloseTag)
            {
                scanner.SkipSymbol();
                scanner.SkipSpaces();
            }

            char ch = scanner.GetSymbol();
            if (ch != '>')
            {
                _errorHandler.Error($"Expected '>' symbol but become:'{ch}'", scanner);
            }
            else
            {
                ret = true;
            }

            return ret;
        }

        private static bool ReadAttributes(XamlObjectNode xamlObjectNode, SymbolScanner scanner, char chNext)
        {
            bool ret = false;
            char breakSymbol;
            string attributeName = string.Empty;
            string attributeValue = string.Empty;

            while (chNext != SymbolScanner.SymbolEndTag)
            {
                scanner.SkipSpaces();

                XamlAttribute attribute = null;
                int lineNumberValue = -1;
                int linePositionValue = -1;
                int lineNumberAttrName = scanner.LineNumber;
                int linePositionAttrName = scanner.CharPosition;
                if (scanner.NextSymbol == SymbolScanner.SymbolCloseTag || scanner.NextSymbol == SymbolScanner.SymbolEndTag)
                {
                    if (attribute != null)
                    {
                        attribute.LineNumberEnd = scanner.LineNumber;
                        attribute.LinePositionEnd = scanner.CharPosition;
                    }

                    chNext = scanner.NextSymbol;
                    break;
                }

                attributeName = scanner.GetName();
                scanner.SkipSpaces();
                breakSymbol = scanner.NextSymbol;

                if (breakSymbol == SymbolScanner.SymbolEq)
                {
                    //skip '='
                    scanner.SkipSymbol();
                    scanner.SkipSpaces();
                    lineNumberValue = scanner.LineNumber;
                    linePositionValue = scanner.CharPosition;
                    attributeValue = scanner.ReadString();
                    chNext = scanner.CheckEndOfLine(scanner.NextSymbol);
                }
                else if (breakSymbol == SymbolScanner.SymbolEndTag)
                {
                    if (attribute != null)
                    {
                        attribute.LineNumberEnd = scanner.LineNumber;
                        attribute.LinePositionEnd = scanner.CharPosition;
                    }

                    chNext = breakSymbol;
                }
                else if (breakSymbol == SymbolScanner.SymbolCloseTag)
                {
                    chNext = scanner.GetSymbol();
                    chNext = scanner.NextSymbol;
                }
                else
                {
                    scanner.SkipSpaces();
                    chNext = scanner.NextSymbol;
                }

                attribute = new XamlAttribute(attributeName, attributeValue)
                                {
                                    LineNumberStart = lineNumberAttrName + 1,
                                    LineNumberEnd = scanner.LineNumber + 1,
                                    LinePositionStart = linePositionAttrName + 1,
                                    LinePositionEnd = scanner.CharPosition ,


                                    LineNumberValueStart = lineNumberValue + 1,
                                    LineNumberValueEnd = scanner.LineNumber + 1,
                                    LinePositionValueStart = linePositionValue + 1,
                                    LinePositionValueEnd = scanner.CharPosition 
                                };

                xamlObjectNode.Attributes.Add(attribute);

                Trace.WriteLine(
                    string.Format(
                        "\t{0}:{1} Attribute Name:{2}, {3}:{4} Value:{5}",
                        lineNumberAttrName + 1,
                        linePositionAttrName + 1,
                        attributeName,
                        lineNumberValue + 1,
                        linePositionValue + 1,
                        attributeValue));
            }

            ret = chNext == SymbolScanner.SymbolEndTag;
            chNext = scanner.CheckEndOfLine(scanner.NextSymbol);
            if (ret)
            {
                chNext = scanner.GetSymbol();
                //chNext = TestNextSymbol(fileStream);
                chNext = scanner.CheckEndOfLine(chNext);
            }

            return ret;
        }

        //public int LineNumber { get; private set; }

        //public int LinePosition { get; private set; }
    }
}
