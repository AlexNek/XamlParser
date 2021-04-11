namespace WpfParser
{
    public class XamlNamespaces : XamlNode<XamlNamespace>
    {
        public override ENodeType NodeType
        {
            get => ENodeType.NamespaceCollection;
        }
    }
}