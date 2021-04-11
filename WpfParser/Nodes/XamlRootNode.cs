using System.Collections.Generic;

namespace WpfParser
{
    public class XamlRootNode : XamlNode<XamlNodeBase>
    {
        public override ENodeType NodeType
        {
            get => ENodeType.Root;
        }

        public XamlMainObjectNode MainObject { get; set; }

        //public XamlResourceCollectionNode Resources { get; set; }

        public IList<XamlNamespace> Namespaces { get; set; }

        public string Path { get; set; }
    }
}