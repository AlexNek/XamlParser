#nullable enable
using System;

namespace WpfParser
{
    public abstract class XamlNodeBase
    {
        public enum ENodeType
        {
            None,

            Root,

            NamespaceCollection,

            Namespace,

            MainObject,

            Attribute,

            //AttributeCollection,

            Object,

            RecourceCollection,

            BindingObject,

            DynamicResourceObject,

            StaticResourceObject,

            TypeExtesionObject,

            StaticExtesionObject
        }

        public virtual string GetDescription(int level)
        {
            return String.Empty;
        }

        public abstract ENodeType NodeType { get; }

        public int? LineNumberEnd { get; set; }

        public int? LineNumberStart { get; set; }

        public int? LinePositionEnd { get; set; }

        public int? LinePositionStart { get; set; }

        public XamlNodeBase? Parent { get; set; }
    }
}
