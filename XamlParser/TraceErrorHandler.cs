using System.Diagnostics;

namespace Parser
{
    public class TraceErrorHandler : IErrorHandler
    {
        public void Error(string text, SymbolScanner scanner)
        {
            Trace.WriteLine(
                string.Format("Parse error:{0} before line {1} position {2} symbol '{3}'",
                    text,
                    scanner.LineNumber,
                    scanner.CharPosition,
                    scanner.NextSymbol));
        }
    }
}
