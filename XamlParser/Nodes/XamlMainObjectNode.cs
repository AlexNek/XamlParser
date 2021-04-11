using System.Xaml;

namespace Parser.Nodes
{
    public class XamlMainObjectNode : XamlObjectNode
    {
        public XamlMainObjectNode()
            : base(null)
        {
        }

        public override XamlNodeBase.ENodeType NodeType
        {
            get => XamlNodeBase.ENodeType.MainObject;
        }

        //public XamlResourceCollectionNode Resources { get; set; }
    }
}
