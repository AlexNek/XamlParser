namespace WpfParser
{
    public class XamlNodeWalker
    {
        private XamlRootNode _root;

        public void Visit(XamlRootNode root, IXamlNodeVisitor visitor)
        {
            _root = root;
            foreach (XamlNamespace xamlNamespace in root.Namespaces)
            {
                Visit(xamlNamespace, visitor);
            }

            //if (root.MainObject.Resources != null)
            //{
            //    Visit(root.MainObject.Resources, visitor);
            //}

            Visit((XamlNodeBase)root.MainObject, visitor);
            _root = null;
        }

        private void Visit(XamlNodeBase node, IXamlNodeVisitor visitor)
        {
            if (visitor.IsAllowedToVisit(node))
            {
                visitor.Visit(node, this);
            }

            XamlObjectNode objectNode = node as XamlObjectNode;
            if (objectNode != null)
            {
                foreach (XamlAttribute attribute in objectNode.Attributes)
                {
                    Visit(attribute, visitor);
                }

                foreach (XamlNodeBase child in objectNode.Children)
                {
                    Visit(child, visitor);
                }
            }
        }

        public XamlRootNode Root
        {
            get
            {
                return _root;
            }
        }
    }
}
