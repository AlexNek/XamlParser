using System;
using System.IO;
using System.Text;

namespace Parser
{
    public class SymbolScanner
    {
        public const char SymbolStartTag = '<';
        public const char SymbolEndTag = '>';
        public const char SymbolCloseTag = '/';

        public const char SymbolCarriageReturn = '\r';
        public const char SymbolNewLine = '\n';

        public const char SymbolFullEnd = '\0';

        public const char SymbolStartComment = '!';

        public const char SymbolEq = '=';

        private const char SymbolApostrophe = '\'';
        private const char SymbolDoubleApostrophe = '"';

        private const char SymbolBackslash = '\\';

        private const char SymbolSpace = ' ';
        private const char SymbolTabulator = '\t';

        private const char SymbolMinus = '-';
        private const char SymbolPoint = '.';
        private const char SymbolUnderscore = '_';

        private const char SymbolColon = ':';


        private readonly StreamReader _fileStream;

        public SymbolScanner(StreamReader fileStream)
        {
            _fileStream = fileStream;
        }

        public char CheckEndOfLine(char ch)
        {
            if (ch == SymbolCarriageReturn)
            {
                //ch='\r', Next = '\n'

                if (ch == NextSymbol)
                {
                    //ch = '\r', Next = '\r' - start with the next symbol

                    //skip '\r'
                    SkipSymbol();
                    ch = NextSymbol;
                }
                else
                {
                    //ch = '\n', Next = '\n' - start with the current symbol
                    ch = NextSymbol;
                    //don't skip '\n' now
                    //SkipSymbol();
                }

                if (ch == SymbolNewLine)
                {
                    //new line detected, skip '\n'
                    SkipSymbol();
                    LineNumber++;
                    CharPosition = 0;
                }

                ch = NextSymbol;
            }

            return ch;
        }

        public string GetName()
        {
            bool IsNotTerminalSymbol(char c)
            {
                return c != SymbolColon && c != SymbolEq && c != SymbolEndTag && c != SymbolSpace && c != SymbolScanner.SymbolCloseTag;
            }

            string ident = GetIdent();
            char breakSymbol = NextSymbol;
            //chNext = TestNextSymbol(fileStream);
            if (IsNotTerminalSymbol(breakSymbol))
            {
                SkipSpaces();
                breakSymbol = NextSymbol;
                if (IsNotTerminalSymbol(breakSymbol))
                {
                    breakSymbol = GetSymbol();
                }
            }

            if (breakSymbol == SymbolColon)
            {
                //SkipSymbol(fileStream);
                string ident2 = GetIdent();
                breakSymbol = NextSymbol;
                ident += SymbolColon + ident2;
                if (breakSymbol != SymbolEq && breakSymbol != SymbolSpace && breakSymbol != SymbolEndTag)
                {
                    SkipSpaces();
                    breakSymbol = GetSymbol();
                }
            }

            return ident;
        }

        public string GetIdent()
        {
            NextSymbol = SymbolFullEnd;
            char ch;
            StringBuilder sb = new StringBuilder();

            if (_fileStream.EndOfStream)
            {
                return string.Empty;
            }
            // read first char

            ch = (char)_fileStream.Read();
            CharPosition++;
            NextSymbol = (char)_fileStream.Peek();
            if (char.IsLetter(ch))
            {
                sb.Append(ch);
            }
            else if (ch == SymbolUnderscore)
            {
                sb.Append(ch);
            }

            if (NextSymbol == SymbolEndTag)
            {
                return sb.ToString();
            }

            while (!_fileStream.EndOfStream)
            {
                ch = NextSymbol;
                if (char.IsLetter(ch))
                {
                    sb.Append(ch);
                }
                else if (ch == SymbolUnderscore)
                {
                    sb.Append(ch);
                }
                else if (ch == SymbolPoint)
                {
                    sb.Append(ch);
                }
                else if (ch == SymbolMinus)
                {
                    sb.Append(ch);
                }
                else if (sb.Length > 0 && char.IsDigit(ch))
                {
                    sb.Append(ch);
                }
                else
                {
                    //NextSymbol = ch;
                    break;
                }

                ch = (char)_fileStream.Read();
                CharPosition++;
                NextSymbol = (char)_fileStream.Peek();
            }

            return sb.ToString();
        }

        public char GetSymbol()
        {
            if (_fileStream.EndOfStream)
            {
                NextSymbol = SymbolFullEnd;
                return SymbolFullEnd;
            }

            char ch = (char)_fileStream.Read();
            CharPosition++;
            NextSymbol = (char)_fileStream.Peek();
            return ch;
        }

        public string ReadComment()
        {
            void AddSymbol(char ch, StringBuilder stringBuilder)
            {
                char chOut = CheckEndOfLine(ch);
                if (chOut != ch)
                {
                    stringBuilder.Append(SymbolCarriageReturn);
                    stringBuilder.Append(SymbolNewLine);
                }
                else
                {
                    stringBuilder.Append(ch);
                }
            }

            StringBuilder sb = new StringBuilder();
            bool start = true;
            if (NextSymbol == SymbolStartComment)
            {
                SkipSymbol();
                while (!_fileStream.EndOfStream)
                {
                    //char ch = (char)_fileStream.Read();
                    //LinePosition++;
                    char ch = GetSymbol();
                    if (ch == SymbolMinus)
                    {
                        //ch = (char)_fileStream.Read();
                        //NextSymbol = (char)_fileStream.Peek();
                        //LinePosition++;
                        ch = GetSymbol();
                        if (ch == SymbolMinus)
                        {
                            if (start)
                            {
                                start = false;
                            }
                            else
                            {
                                char chNext = NextSymbol;
                                if (chNext == SymbolEndTag)
                                {
                                    SkipSymbol();
                                    //ch = (char)_fileStream1.Read();
                                    break;
                                }

                                sb.Append(ch);
                            }

                            //ch = (char)_fileStream.Read();
                            //LinePosition++;
                            ch = GetSymbol();
                            AddSymbol(ch, sb);
                        }
                        else
                        {
                            AddSymbol(ch, sb);
                        }
                    }
                    else
                    {
                        AddSymbol(ch, sb);
                    }
                }
            }

            NextSymbol = (char)_fileStream.Peek();
            return sb.ToString();
        }

        public string ReadString()
        {
            StringBuilder sb = new StringBuilder();
            SkipSpaces();
            char ch = GetSymbol();
            char stringStart = SymbolFullEnd;
            if (ch == SymbolDoubleApostrophe || ch == SymbolApostrophe)
            {
                stringStart = ch;
            }

            if (stringStart != SymbolFullEnd)
            {
                while (!_fileStream.EndOfStream)
                {
                    ch = (char)_fileStream.Read();
                    CharPosition++;
                    if (ch == stringStart)
                    {
                        NextSymbol = (char)_fileStream.Peek();
                        break;
                    }

                    if (ch == SymbolBackslash)
                    {
                        SkipSymbol();
                        ch = GetSymbol();
                        sb.Append(SymbolBackslash);
                        sb.Append(ch);
                    }

                    //else
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        public string ReadText(char startSymbol)
        {
            if (NextSymbol == SymbolStartTag || startSymbol == SymbolStartTag)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            int lastTextPosition = 0;
            sb.Append(startSymbol);
            while (!_fileStream.EndOfStream)
            {
                char ch = (char)_fileStream.Read();
                CharPosition++;
                NextSymbol = (char)_fileStream.Peek();
                if (NextSymbol == SymbolStartTag)
                {
                    sb.Append(ch);
                    break;
                }

                if (ch == SymbolBackslash)
                {
                    SkipSymbol();
                    ch = GetSymbol();
                    sb.Append(SymbolBackslash);
                    sb.Append(ch);
                }

                sb.Append(ch);
                lastTextPosition = CharPosition;
                char chOut = CheckEndOfLine(ch);
                if (chOut != ch)
                {
                    if (NextSymbol == SymbolStartTag)
                    {
                        //sb.Append(SymbolCarriageReturn);
                        sb.Append(SymbolNewLine);
                        break;
                    }

                    sb.Append(chOut);
                }
            }

            LastTextLineNumberEnd = LineNumber;
            LastTextCharPositionEnd = CharPosition;

            //remove last newLine symbols
            string text = sb.ToString();
            int lastIndexOfNewLine = text.LastIndexOf("\r\n");
            if (lastIndexOfNewLine == text.Length - 2)
            {
                LastTextLineNumberEnd--;
                LastTextCharPositionEnd = lastTextPosition - 1;
                return text.Substring(0, lastIndexOfNewLine);
            }

            return text;
        }

        public void SkipSpaces()
        {
            while (!_fileStream.EndOfStream)
            {
                char ch = NextSymbol;
                if (ch != SymbolSpace && ch != SymbolTabulator && ch != SymbolCarriageReturn)
                {
                    //NextSymbol = ch;
                    break;
                }

                if (ch == SymbolCarriageReturn)
                {
                    CheckEndOfLine(ch);
                }
                else
                {
                    ch = (char)_fileStream.Read();
                    CharPosition++;
                    NextSymbol = (char)_fileStream.Peek();
                }
            }
        }

        public void SkipSymbol()
        {
            if (!_fileStream.EndOfStream)
            {
                char ch = (char)_fileStream.Read();
                CharPosition++;
                NextSymbol = (char)_fileStream.Peek();
            }
        }

        public int CharPosition { get; private set; }

        public StreamReader FileStream
        {
            get
            {
                return _fileStream;
            }
        }

        public int LastTextCharPositionEnd { get; private set; }

        public int LastTextLineNumberEnd { get; private set; }

        public int LineNumber { get; private set; }

        public char NextSymbol { get; private set; }

        
    }
}
