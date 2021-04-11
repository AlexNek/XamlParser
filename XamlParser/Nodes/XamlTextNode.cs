namespace Parser.Nodes
{
    public class XamlTextNode : XamlNodeBase
    {
        public XamlTextNode(string text)
        {
            Value = text;
        }

        public override ENodeType NodeType
        {
            get
            {
                return ENodeType.TextObject;
            }
        }

        public string Value { get; private set; }
    }
}
