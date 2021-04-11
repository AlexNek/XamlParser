using System.Text;
using System.Xaml;

namespace WpfParser.Nodes
{
    public abstract class XamlExtensionObjectNodeBase : XamlObjectNode
    {
        public XamlExtensionObjectNodeBase(XamlType xamlType)
            : base(xamlType)
        {
        }

        public override string GetDescription(int level)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Name))
            {
                sb.AppendFormat("{0}=\"", Name);
            }

            sb.Append("{" + ExtensionName + " ");
            foreach (XamlAttribute node in Attributes)
            {
                if (node.Name == "_PositionalParameters")
                {
                    string value = GetValue(node, level);
                    sb.Append(value);
                }
                else
                {
                    sb.AppendFormat(", {0}={1}", node.Name, node.Value);
                }
            }

            if (!string.IsNullOrEmpty(Name))
            {
                sb.Append("}\"");
            }
            else
            {
                sb.Append("}");
            }

            return sb.ToString();
        }

        private string GetValue(XamlAttribute node, int level)
        {

            if (node.Value is XamlExtensionObjectNodeBase extensionObjectNodeBase)
            {
                string description;
                if (string.IsNullOrEmpty(Name))
                {
                    description = string.Format(" {0}", extensionObjectNodeBase.GetDescription(level));
                }
                else
                {
                    description = string.Format(" {0}{1}", Name, extensionObjectNodeBase.GetDescription(level));
                }

                return description;
            }
            else
            {
                if (node.Name == "_PositionalParameters")
                {
                    return node.Value?.ToString();
                }

                return string.Format(", {0}={1}", node.Name, node.Value);
            }
        }

        protected abstract string ExtensionName { get; }
    }
}