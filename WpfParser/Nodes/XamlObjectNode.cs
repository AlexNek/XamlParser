using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace WpfParser
{
    public class XamlObjectNode : XamlNode<XamlNodeBase>
    {
        private readonly bool _isExtension;

        public XamlObjectNode(XamlType xamlType)
        {
            XamlType = xamlType;
        }

        public override string GetDescription(int level)
        {
            StringBuilder sb = new StringBuilder();
            string typeName = XamlType.Name;
            if (NodeType == ENodeType.RecourceCollection)
            {
                typeName = "Resources";
                if (Parent is XamlObjectNode parent)
                {
                    typeName = $"{parent.XamlType.Name}.Resources";
                }
                sb.AppendFormat("<{0} ", typeName);
            }
            else
            {
                if (Name != null)
                {
                    sb.AppendFormat("<{0} {1}", typeName, Name);
                }
                else
                {
                    sb.AppendFormat("<{0} ", typeName);
                }

                foreach (XamlAttribute attribute in Attributes)
                {
                    sb.Append(attribute.GetDescription(level));
                }
            }

            if (Children.Count == 0)
            {
                sb.AppendLine("/>");
            }
            else
            {
                sb.AppendLine(">");
                foreach (XamlNodeBase child in Children)
                {
                    AddPrefixSpaces(level + 1, sb);

                    if (child.LineNumberStart.HasValue)
                    {
                        sb.AppendLine($"{child.LineNumberStart}:{child.LinePositionStart}-{child.LineNumberEnd}:{child.LinePositionEnd}");
                    }

                    AddPrefixSpaces(level + 1, sb);
                    sb.Append(child.GetDescription(level + 1));
                }
                AddPrefixSpaces(level, sb);
                //if (NodeType == ENodeType.RecourceCollection)
                sb.AppendFormat("</{0}>", typeName);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void AddPrefixSpaces(int level, StringBuilder sb)
        {
            for (int i = 0; i < level; i++)
            {
                sb.Append("\t");
            }
        }

        public List<XamlAttribute> Attributes { get; } = new List<XamlAttribute>();

        public string Name { get; set; }

        public override ENodeType NodeType
        {
            get => ENodeType.Object;
        }

        public XamlType XamlType { get; }

        public bool IsExtension
        {
            get
            {
                bool ret = false;
                switch (NodeType)
                {
                    case ENodeType.None:
                        break;
                    case ENodeType.Root:
                    case ENodeType.NamespaceCollection:
                    case ENodeType.Namespace:
                    case ENodeType.MainObject:
                    case ENodeType.Attribute:
                    case ENodeType.Object:
                    case ENodeType.RecourceCollection:
                        break;
                    case ENodeType.BindingObject:
                    case ENodeType.DynamicResourceObject:
                    case ENodeType.StaticResourceObject:
                    case ENodeType.TypeExtesionObject:
                    case ENodeType.StaticExtesionObject:
                        ret = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return ret;
            }
        }
    }
}
