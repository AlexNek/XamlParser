using System.Xaml;

namespace WpfParser
{
    public class XamlMainObjectNode : XamlObjectNode
    {
        public XamlMainObjectNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override ENodeType NodeType
        {
            get => ENodeType.MainObject;
        }

        public XamlResourceCollectionNode Resources { get; set; }
    }
}
