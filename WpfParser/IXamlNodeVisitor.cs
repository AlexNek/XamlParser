namespace WpfParser
{
    public interface IXamlNodeVisitor
    {
        bool IsAllowedToVisit(XamlNodeBase node);

        void Visit(XamlNodeBase node, XamlNodeWalker xamlNodeWalker);
    }
}