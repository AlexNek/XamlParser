using System;
using System.Xaml;

namespace Parser
{
    public class ExceptionErrorHandler : IErrorHandler
    {
        private const char EOF = '\uffff';


        public void Error(string text, SymbolScanner scanner)
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            string nextSymbol = scanner.NextSymbol.ToString();

            if (scanner.NextSymbol == EOF)
            {
                nextSymbol = "EOF";
            }
            throw new XamlParseException(
                string.Format("Parse error:{0} at/before line {1}  position {2} and before symbol '{3}'",
                    text,
                    scanner.LineNumber+1,
                    scanner.CharPosition+1,
                    nextSymbol));
        }
    }
}
