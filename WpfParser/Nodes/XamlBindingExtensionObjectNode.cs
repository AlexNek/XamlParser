using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlBindingExtensionObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "Binding";

        public XamlBindingExtensionObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.BindingObject;
        }

        protected override string ExtensionName
        {
            get
            {
                return "Binding";
            }
        }
    }
}
