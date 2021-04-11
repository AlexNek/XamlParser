#nullable enable
using System;

namespace Parser.Nodes
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

            TypeExtensionObject,

            StaticExtensionObject,

            Comment,

            TextObject
        }

        [Flags]
        public enum EState
        {
            None = 0,

            EndTagPresent = 0x01,

            Closed = 0x02,

            TextNodePresent = 0x04,
        }

        private EState _state;

        protected XamlNodeBase()
        {
            Id = Tools.GetObjId(this);
            //Parent = null;
        }

        public virtual string GetDescription(int level)
        {
            return String.Empty;
        }

        public bool IsState(EState checkState)
        {
            return (_state & checkState) == checkState;
        }

        public virtual void SetState(EState newState)
        {
            _state |= newState;
        }

        public abstract ENodeType NodeType { get; }

        public long Id { get; set; }

        public int? LineNumberEnd { get; set; }

        public int? LineNumberStart { get; set; }

        public int? LinePositionEnd { get; set; }

        public int? LinePositionStart { get; set; }

        public XamlNodeBase? Parent { get; set; }

        public EState State
        {
            get
            {
                return _state;
            }
        }
    }
}
