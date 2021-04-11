using System.Collections.Generic;

namespace Parser.Nodes
{
    public abstract class XamlNode<T> : XamlNodeBase
        where T : XamlNodeBase
    {
        public void AddChild(T item)
        {
            item.Parent = this;
            Children.Add(item);
        }

        public List<T> Children { get; } = new List<T>();
    }
}
