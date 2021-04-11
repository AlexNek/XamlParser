namespace Parser
{
    public interface IErrorHandler
    {
        void Error(string text, SymbolScanner scanner);
    }
}
