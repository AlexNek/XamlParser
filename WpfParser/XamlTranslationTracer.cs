using System.Diagnostics;

namespace WpfParser
{
    public class XamlTranslationTracer : IXamlTranslationWriter
    {
        public void WriteEntering(XamlNodeBase node)
        {
            Trace.WriteLine($"{node.LineNumberStart}:{node.LinePositionStart}-{node.LineNumberEnd}:{node.LinePositionEnd} {node.NodeType}");

            if (node is XamlObjectNode objectNode)
            {
                Trace.WriteLine($" {objectNode.XamlType.Name}-{objectNode.Name}");
            }
        }

        public void WriteTranslation(XamlAttribute attribute)
        {
            //{ attribute.XamlType.PreferredXamlNamespace}_: _{ attribute.XamlType.Name} - all the same
            Trace.WriteLine(
                $"({attribute.DeclaringType?.Name},{attribute.XamlType.UnderlyingType})W={attribute.IsWritePublic} {attribute.Name}={attribute.Value?.GetType()}/{attribute.Value}");
        }
    }
}