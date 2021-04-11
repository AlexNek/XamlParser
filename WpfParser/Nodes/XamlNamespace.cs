namespace WpfParser
{
    public class XamlNamespace : XamlNodeBase
    {
        public XamlNamespace(string prefix, string namespaceName)
        {
            Prefix = prefix;
            Namespace = namespaceName;
        }

        public override string GetDescription(int level)
        {
            if (string.IsNullOrEmpty(Prefix))
            {
                return $"xmlns=\"{Namespace}\"";
            }

            return $"xmlns:{Prefix}=\"{Namespace}\"";
        }

        public string Namespace { get; private set; }

        public override ENodeType NodeType
        {
            get => ENodeType.Namespace;
        }

        public string Prefix { get; private set; }
    }
}
