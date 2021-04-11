using System;
using System.Xaml;

namespace Parser.Nodes
{
    public class XamlAttribute : XamlNodeBase
    {
        public XamlAttribute(string name, object value)
        {
            //XamlType = xamlType;
            Name = name;
            Value = value;
        }

        public override string GetDescription(int level)
        {
            //if (Value is XamlExtensionObjectNodeBase extensionObjectNodeBase)
            //{
            //    string description = string.Format(" {0}=\"{1}\"", Name, extensionObjectNodeBase.GetDescription(level));
            //    return description;
            //}

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

        public int LineNumberValueStart { get; set; }

        public int LinePositionValueStart { get; set; }

        public int LineNumberValueEnd { get; set; }

        public int LinePositionValueEnd { get; set; }

        //public XamlType XamlType { get; }
    }
}
