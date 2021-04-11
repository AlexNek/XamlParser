using System;
using System.Xaml;

using WpfParser.Nodes;

namespace WpfParser
{
    public class XamlAttribute : XamlNodeBase
    {
        public XamlAttribute(XamlType xamlType, string name, object value)
        {
            XamlType = xamlType;
            Name = name;
            Value = value;
        }

        public override string GetDescription(int level)
        {
            if (Value is XamlExtensionObjectNodeBase extensionObjectNodeBase)
            {
                string description = string.Format(" {0}=\"{1}\"", Name, extensionObjectNodeBase.GetDescription(level));
                return description;
            }

            return String.Format(" {0}=\"{1}\"", Name, Value);
        }

        public XamlType DeclaringType { get; set; }

        public bool IsWritePublic { get; set; }

        public string Name { get; }

        public override ENodeType NodeType
        {
            get => ENodeType.Attribute;
        }

        public object Value { get; }

        public XamlType XamlType { get; }
    }
}
