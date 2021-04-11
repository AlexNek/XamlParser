using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlStatResObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "StaticResource";

        public XamlStatResObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.StaticResourceObject;
        }

        protected override string ExtensionName
        {
            get
            {
                return "StaticResource";
            }
        }
    }
}
