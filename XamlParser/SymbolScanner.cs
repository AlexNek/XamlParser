using System;
using System.IO;
using System.Text;

namespace Parser
{
    public class SymbolScanner
    {
        public const char SymbolFullEnd = '\0';

        public const char SymbolCarriageReturn = '\r';

        public const char SymbolNewLine = '\n';

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

        public string GetIdent()
        {
            NextSymbol = SymbolFullEnd;
            char ch;
            StringBuilder sb = new StringBuilder();

            if (_fileStream.EndOfStream)
            {
                return String.Empty;
            }
            // read first char

            ch = (char)_fileStream.Read();
            CharPosition++;
            NextSymbol = (char)_fileStream.Peek();
            if (Char.IsLetter(ch))
            {
                sb.Append(ch);
            }
            else if (ch == '_')
            {
                sb.Append(ch);
            }

            if (NextSymbol == '>')
            {
                return sb.ToString();
            }

            while (!_fileStream.EndOfStream)
            {
                ch = NextSymbol;
                if (Char.IsLetter(ch))
                {
                    sb.Append(ch);
                }
                else if (ch == '_')
                {
                    sb.Append(ch);
                }
                else if (ch == '.')
                {
                    sb.Append(ch);
                }
                else if (ch == '-')
                {
                    sb.Append(ch);
                }
                else if (sb.Length > 0 && Char.IsDigit(ch))
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
                    stringBuilder.Append("\r\n");
                }
                else
                {
                    stringBuilder.Append(ch);
                }
            }

            StringBuilder sb = new StringBuilder();
            bool start = true;
            if (NextSymbol == '!')
            {
                SkipSymbol();
                while (!_fileStream.EndOfStream)
                {
                    //char ch = (char)_fileStream.Read();
                    //LinePosition++;
                    char ch = GetSymbol();
                    if (ch == '-')
                    {
                        //ch = (char)_fileStream.Read();
                        //NextSymbol = (char)_fileStream.Peek();
                        //LinePosition++;
                        ch = GetSymbol();
                        if (ch == '-')
                        {
                            if (start)
                            {
                                start = false;
                            }
                            else
                            {
                                char chNext = NextSymbol;
                                if (chNext == '>')
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
            if (ch == '"' || ch == '\'')
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

                    if (ch == '\\')
                    {
                        SkipSymbol();
                        ch = GetSymbol();
                        sb.Append('\\');
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
            if (NextSymbol == '<' || startSymbol == '<')
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
                if (NextSymbol == '<')
                {
                    sb.Append(ch);
                    break;
                }

                if (ch == '\\')
                {
                    SkipSymbol();
                    ch = GetSymbol();
                    sb.Append('\\');
                    sb.Append(ch);
                }

                sb.Append(ch);
                lastTextPosition = CharPosition;
                char chOut = CheckEndOfLine(ch);
                if (chOut != ch)
                {
                    if (NextSymbol == '<')
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
                if (ch != ' ' && ch != '\t' && ch != SymbolCarriageReturn)
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
