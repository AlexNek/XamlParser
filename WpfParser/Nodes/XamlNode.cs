using System.Collections.Generic;

namespace WpfParser
{
    public abstract class XamlNode<T> : XamlNodeBase
    {
        public List<T> Children { get; } = new List<T>();
    }
}