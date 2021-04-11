using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlStaticExtensionObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "StaticExtension";

        public XamlStaticExtensionObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.StaticExtesionObject;
        }

        protected override string ExtensionName
        {
            get
            {
                return "x:Static";
            }
        }
    }
}
