using System.Diagnostics;

namespace WpfParser
{
    public class XamlNodeTracer
    {
        public static void TraceNode(XamlRootNode root)
        {
            Trace.WriteLine("***root***");
            Trace.WriteLine(root.Path);

            Trace.WriteLine("***namespaces***");
            foreach (XamlNamespace xamlNamespace in root.Namespaces)
            {
                TraceNode(xamlNamespace);
            }

            TraceNode(root.MainObject);

            foreach (XamlNodeBase child in root.Children)
            {
                TraceNode(child);
            }

            //if (root.MainObject.Resources != null)
            //{
            //    Trace.WriteLine("***resources***");
            //    foreach (XamlNodeBase resource in root.MainObject.Resources.Children)
            //    {
            //        TraceNode(resource);
            //    }
            //}
            Trace.WriteLine("***End***");
        }

        private static void TraceNode(XamlNodeBase node)
        {
            if (node.LineNumberStart.HasValue)
            {
                Trace.WriteLine($"{node.LineNumberStart}:{node.LinePositionStart}-{node.LineNumberEnd}:{node.LinePositionEnd}");
            }
            Trace.WriteLine(node.GetDescription(0));
        }
    }
}
