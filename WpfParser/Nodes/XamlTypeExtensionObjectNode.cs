using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlTypeExtensionObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "TypeExtension";

        public XamlTypeExtensionObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.TypeExtesionObject;
        }

        protected override string ExtensionName
        {
            get
            {
                return "x:Type";
            }
        }
    }
}