using System.Xaml;

namespace WpfParser.Nodes
{
    public class XamlDynResObjectNode : XamlExtensionObjectNodeBase
    {
        public const string Ident = "DynamicResource";

        public XamlDynResObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.DynamicResourceObject;
        }

        protected override string ExtensionName
        {
            get
            {
                return "DynamicResource";
            }
        }
    }
}