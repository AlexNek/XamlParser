using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xaml;
using System.Xml;

using CommonData;

using WpfParser.Nodes;

namespace WpfParser
{
    public class XamlFileParser
    {
        private const string ResourcesSectionName = "Resources";

        //private readonly ProjectDataItemBase _selectedProject;

        //public XamlFileParser(ProjectDataItemBase selectedProject)
        //{
        //    _selectedProject = selectedProject;
        //}

        //public async Task CollectStringsAsync()
        //{
        //    //_selectedProject.UiFiles.Clear();
        //    foreach (FileDataItemBase fileDataItem in _selectedProject.AllFiles)
        //    {
        //        if (fileDataItem.ItemType == FileDataItemBase.EItemType.XamlPage
        //            || fileDataItem.ItemType == FileDataItemBase.EItemType.XamlApplicationDefinition)
        //        {
        //            fileDataItem.Translations.Clear();
        //            XamlRootNode root = Parse(fileDataItem.FullPath);
        //            //XamlNodeTracer.TraceNode(root);
        //            XamlNodeWalker nodeWalker = new XamlNodeWalker();

        //            XamlTranslationWriter translationTracer = new XamlTranslationWriter(fileDataItem);
        //            XamlCodeTextVisitor visitor = new XamlCodeTextVisitor(translationTracer);
        //            nodeWalker.Visit(root, visitor);
        //        }

        //        await Task.Delay(10);
        //    }
        //}

        public static XamlRootNode Parse(string fullPath)
        {
            int xamlObjectCount = 0;
            XamlRootNode root = new XamlRootNode();
            
            try
            {
                //XamlNamespaces namespaces = new XamlNamespaces();
                //root.Children.Add(namespaces);
                Stack<XamlObjectNode> objStack = new Stack<XamlObjectNode>();
                Stack<XamlStartMemberTempData> memberStack = new Stack<XamlStartMemberTempData>();
                Stack<XamlExtensionObjectNodeBase> extensionsObjStack = new Stack<XamlExtensionObjectNodeBase>();

                XamlStartMemberTempData startMemberTempData = null;
                XamlValueTempData valueTempData = null;
                //bool wasGetObject = false;

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.IgnoreComments = false;

                XamlXmlReaderSettings xamlReaderSettings = new XamlXmlReaderSettings();
                xamlReaderSettings.IgnoreUidsOnPropertyElements = false;
                xamlReaderSettings.ProvideLineInfo = true;

                XmlReader xmlReader = XmlReader.Create(fullPath, readerSettings);
                XamlXmlReader xamlReader = new XamlXmlReader(xmlReader, xamlReaderSettings);

                xamlReader.Read();
                while (!xamlReader.IsEof)
                {
                    XamlNodeType nodeType = xamlReader.NodeType;
                    Trace.WriteLine($"--{xamlReader.LineNumber}:{xamlReader.LinePosition}.{nodeType}--");
                    switch (nodeType)
                    {
                        case XamlNodeType.None:
                            break;
                        case XamlNodeType.StartObject:
                            {
                                //XamlBindingObjectNode
                                if (xamlObjectCount == 0)
                                {
                                    XamlMainObjectNode mainObject = new XamlMainObjectNode(xamlReader.Type)
                                                                        {
                                                                            LineNumberStart = xamlReader.LineNumber,
                                                                            LinePositionStart = xamlReader.LinePosition
                                                                        };
                                    root.MainObject = mainObject;
                                    objStack.Push(mainObject);
                                }
                                else
                                {
                                    XamlObjectNode obj;
                                    if (xamlReader.Type.Name == XamlBindingExtensionObjectNode.Ident)
                                    {
                                        obj = new XamlBindingExtensionObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else if (xamlReader.Type.Name == XamlTemplateBindingExtensionObjectNode.Ident)
                                    {
                                        obj = new XamlTemplateBindingExtensionObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else if (xamlReader.Type.Name == XamlDynResObjectNode.Ident)
                                    {
                                        obj = new XamlDynResObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else if (xamlReader.Type.Name == XamlTypeExtensionObjectNode.Ident)
                                    {
                                        obj = new XamlTypeExtensionObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else if (xamlReader.Type.Name == XamlStatResObjectNode.Ident)
                                    {
                                        obj = new XamlStatResObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else if (xamlReader.Type.Name == XamlStaticExtensionObjectNode.Ident)
                                    {
                                        obj = new XamlStaticExtensionObjectNode(xamlReader.Type);
                                        extensionsObjStack.Push((XamlExtensionObjectNodeBase)obj);
                                    }
                                    else
                                    {
                                        obj = new XamlObjectNode(xamlReader.Type);
                                        //if (memberStack.Count == 0)
                                        {
                                            if (objStack.Count > 0)
                                            {
                                                XamlObjectNode objNode = objStack.Peek();
                                                obj.Parent = objNode;
                                                objNode.Children.Add(obj);
                                            }
                                        }
                                        //else
                                        //{
                                        //    XamlStartMemberTempData memberTempData = memberStack.Peek();
                                        //    XamlObjectNode obj2 = new XamlObjectNode(memberTempData.Type){Name = memberTempData.Name};
                                        //    obj2.Children.Add(obj);
                                        //}
                                    }

                                    obj.LineNumberStart = xamlReader.LineNumber;
                                    obj.LinePositionStart = xamlReader.LinePosition;

                                    //root.Children.Add(obj);
                                    objStack.Push(obj);
                                }

                                XamlSchemaContext schemaContext = xamlReader.SchemaContext;
                                XamlType readerType = xamlReader.Type;
                                Trace.WriteLine($"{xamlReader.Type},{xamlReader.Type.ItemType}---{readerType.Name}");
                                xamlObjectCount++;
                            }
                            break;
                        case XamlNodeType.GetObject:
                            {
                                XamlObjectNode obj;

                                //if (startMemberTempData.Name == ResourcesSectionName)
                                //{
                                //    obj = root.Resources;
                                //}
                                //else
                                //{
                                //    obj = new XamlObjectNode(startMemberTempData.Type) { Name = startMemberTempData.Name };
                                //    if (objStack.Count > 0)
                                //    {
                                //        XamlObjectNode objNode = objStack.Peek();
                                //        objNode.Children.Add(obj);
                                //    }
                                //}

                                //obj.LinePositionStart = xamlReader.LinePosition;
                                //obj.LineNumberStart = xamlReader.LineNumber;
                                //objStack.Push(obj);
                            }
                            //wasGetObject = true;
                            break;
                        case XamlNodeType.EndObject:
                            {
                                if (objStack.Count > 0)
                                {
                                    //remove current object
                                    XamlObjectNode objNode = objStack.Pop();

                                    objNode.LineNumberEnd = xamlReader.LineNumber;
                                    objNode.LinePositionEnd = xamlReader.LinePosition;
                                    //if (objNode.NodeType == XamlNodeBase.ENodeType.BindingObject)
                                    //{
                                    //    bindingStack
                                    //}
                                }
                                else
                                {
                                    //for breakpoint
                                   Trace.WriteLine("Error obj stack is empty");
                                }
                            }
                            break;
                        case XamlNodeType.StartMember:
                            {
                                //if (wasGetObject)
                                //{
                                //    break;
                                //}
                                XamlMember member = xamlReader.Member;
                                XamlType memberType = member.Type;
                                XamlType declaringType = member.DeclaringType;
                                if (declaringType != null && member.Name == ResourcesSectionName)
                                {
                                    XamlResourceCollectionNode xamlResourceCollectionNode = new XamlResourceCollectionNode(member.Type);
                                    //root.Resources = new List<XamlNodeBase>();
                                    root.MainObject.Resources = xamlResourceCollectionNode;

                                    xamlResourceCollectionNode.Name = member.Name;
                                    xamlResourceCollectionNode.LinePositionStart = xamlReader.LinePosition;
                                    xamlResourceCollectionNode.LineNumberStart = xamlReader.LineNumber;

                                    xamlResourceCollectionNode.Parent = root.MainObject;
                                    root.MainObject.Children.Add(xamlResourceCollectionNode);
                                    objStack.Push(xamlResourceCollectionNode);
                                }

                                IList<string> xamlNamespaces = member.GetXamlNamespaces();
                                string namespacesStr = String.Join(";", xamlNamespaces);
                                Trace.WriteLine($"{namespacesStr} ___ {memberType}({declaringType}).{member.Name} === {member}");
                                startMemberTempData = new XamlStartMemberTempData();
                                startMemberTempData.Type = member.Type;
                                startMemberTempData.Name = member.Name;
                                startMemberTempData.DeclaringType = member.DeclaringType;
                                startMemberTempData.IsWritePublic = member.IsWritePublic;
                                startMemberTempData.LineNumberStart = xamlReader.LineNumber;
                                startMemberTempData.LinePositionStart = xamlReader.LinePosition;
                                valueTempData = null;
                                memberStack.Push(startMemberTempData);
                            }

                            break;
                        case XamlNodeType.EndMember:
                            {
                                if (startMemberTempData != null)
                                {
                                    if (valueTempData != null)
                                    {
                                        //remove last member
                                        if (memberStack.Count > 0)
                                        {
                                            XamlStartMemberTempData memberTempData = memberStack.Pop();
                                        }

                                        if (startMemberTempData.Name == "base")
                                        {
                                            root.Path = valueTempData.Value?.ToString();
                                        }
                                        else
                                        {
                                            XamlAttribute attribute = new XamlAttribute(
                                                startMemberTempData.Type,
                                                startMemberTempData.Name,
                                                valueTempData.Value);

                                            attribute.LineNumberStart = startMemberTempData.LineNumberStart;
                                            attribute.LinePositionStart = startMemberTempData.LinePositionStart;
                                            attribute.LineNumberEnd = xamlReader.LineNumber;
                                            attribute.LinePositionEnd = xamlReader.LinePosition;
                                            attribute.DeclaringType = startMemberTempData.DeclaringType;
                                            attribute.IsWritePublic = startMemberTempData.IsWritePublic;
                                            if (objStack.Count > 0)
                                            {
                                                XamlObjectNode objNode = objStack.Peek();

                                                AddAttribute(attribute, objNode);
                                            }
                                        }

                                        valueTempData = null;
                                    }
                                    else
                                    {
                                        AddAttributesWithObjValue(memberStack, extensionsObjStack, objStack);
                                    }

                                    startMemberTempData = null;
                                }
                                else
                                {
                                    AddAttributesWithObjValue(memberStack, extensionsObjStack, objStack);
                                }
                            }

                            break;
                        case XamlNodeType.Value:
                            {
                                object value = xamlReader.Value;
                                Trace.WriteLine($"{value.GetType()}-{value}");
                                valueTempData = new XamlValueTempData { Type = value.GetType(), Value = value };
                            }

                            break;
                        case XamlNodeType.NamespaceDeclaration:
                            {
                                if (root.Namespaces == null)
                                {
                                    root.Namespaces = new List<XamlNamespace>();
                                }

                                NamespaceDeclaration namespaceDeclaration = xamlReader.Namespace;
                                XamlNamespace ns = new XamlNamespace(namespaceDeclaration.Prefix, namespaceDeclaration.Namespace)
                                                       {
                                                           LineNumberStart = xamlReader.LineNumber, LinePositionStart = xamlReader.LinePosition
                                                       };
                                Trace.WriteLine($"{namespaceDeclaration.Prefix}:{namespaceDeclaration.Namespace} ");
                                root.Namespaces.Add(ns);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    bool isNext = xamlReader.Read();
                }
                //XamlReader builderReader = ActivityXamlServices.CreateBuilderReader(xamlReader);
                //ActivityBuilder ab = XamlServices.Load(builderReader) as ActivityBuilder;

                //XmlWriterSettings writerSettings = new XmlWriterSettings { Indent = true };
                //XmlWriter xmlWriter = XmlWriter.Create(File.OpenWrite(args[1]), writerSettings);
                //XamlXmlWriter xamlWriter = new XamlXmlWriter(xmlWriter, new XamlSchemaContext());
                //XamlServices.Save(new ViewStateCleaningWriter(ActivityXamlServices.CreateBuilderWriter(xamlWriter)), ab);

                //Console.WriteLine("{0} written without viewstate information", args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception encountered {0}", ex);
            }

           
            return root;
        }

        private static void AddAttribute(XamlAttribute attribute, XamlObjectNode objNode)
        {
            if (attribute.Name != "_UnknownContent")
            {
                attribute.Parent = objNode;
                objNode.Attributes.Add(attribute);
            }
        }

        private static void AddAttributesWithObjValue(
            Stack<XamlStartMemberTempData> memberStack,
            Stack<XamlExtensionObjectNodeBase> extensionsObjStack,
            Stack<XamlObjectNode> objStack)
        {
            if (memberStack.Count > 0)
            {
                XamlExtensionObjectNodeBase extensionObjectNode = null;
                if (extensionsObjStack.Count > 0)
                {
                    extensionObjectNode = extensionsObjStack.Pop();
                }
                XamlStartMemberTempData memberTempData = memberStack.Pop();
                //if (extensionObjectNode != null)
                {
                    XamlAttribute attribute = new XamlAttribute(
                                                  memberTempData.Type,
                                                  memberTempData.Name,
                                                  extensionObjectNode)
                                                  {
                                                      LineNumberStart = memberTempData.LineNumberStart,
                                                      LinePositionStart = memberTempData.LinePositionStart
                                                  };

                    if (objStack.Count > 0)
                    {
                        XamlObjectNode objNode = objStack.Peek();
                        //if (attribute.Value != null)
                        {
                            //if (!attribute.Value.Equals(objNode))
                            {
                                AddAttribute(attribute, objNode);
                            }
                        }
                        //else
                        //{
                        //    Trace.WriteLine("Skipped attribute with NULL value");
                        //}

                        if (objNode.IsExtension)
                        {
                            if (extensionsObjStack.Count > 0)
                            {
                                extensionObjectNode = extensionsObjStack.Peek();
                                if (!extensionObjectNode.Equals(objNode))
                                {
                                    extensionsObjStack.Push((XamlExtensionObjectNodeBase)objNode);
                                }
                            }
                            else
                            {
                                extensionsObjStack.Push((XamlExtensionObjectNodeBase)objNode);
                            }
                        }
                        else
                        {
                            ;
                        }
                    }
                }
                //else
                //{
                //    //if (objStack.Count > 0)
                //    //{
                //    //    XamlObjectNode objNode = objStack.Peek();
                //    //    XamlObjectNode xamlNodeBase = new XamlObjectNode(memberTempData.Type){Name = memberTempData.Name };
                //    //    objNode.Children.Add(xamlNodeBase);
                //    //    //objStack.Push(xamlNodeBase);
                //    //}
                //}
            }
        }

        private class XamlStartMemberTempData
        {
            public string Name { get; set; }

            public XamlType Type { get; set; }

            public int? LineNumberStart { get; set; }

            public int? LinePositionStart { get; set; }

            public XamlType DeclaringType { get; set; }

            public bool IsWritePublic { get; set; }
        }

        private class XamlValueTempData
        {
            public Type Type { get; set; }

            public object Value { get; set; }
        }
    }
}
