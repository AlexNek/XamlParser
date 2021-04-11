using System.Text;
using System.Xaml;

namespace WpfParser
{
    public class XamlResourceCollectionNode : XamlObjectNode
    {
        public XamlResourceCollectionNode(XamlType xamlType)
            : base(xamlType)
        {
        }

        public string Name { get; set; }

        public override ENodeType NodeType
        {
            get => ENodeType.RecourceCollection;
        }
    }
}
