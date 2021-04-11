using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlTemplateBindingExtensionObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "TemplateBinding";

        public XamlTemplateBindingExtensionObjectNode(XamlType xamlType)
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
                return "TemplateBinding";
            }
        }
    }
}
