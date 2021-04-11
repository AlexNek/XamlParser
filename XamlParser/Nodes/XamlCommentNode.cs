namespace Parser.Nodes
{
    public class XamlCommentNode : XamlObjectNode
    {
        public XamlCommentNode()
        {
            Name = "@comment";
        }

        public string Comment { get; set; }

        public override ENodeType NodeType
        {
            get => ENodeType.Comment;
        }
    }
}
