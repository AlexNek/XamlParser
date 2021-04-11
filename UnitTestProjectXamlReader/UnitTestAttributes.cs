using System;

using NUnit.Framework;

using Parser;
using Parser.Nodes;

namespace UnitTestProjectXamlReader
{
    public class UnitTestAttributes : UnitTestParserBase
    {
        protected static void CheckTextRangeValue(string prefix, int? expectedLs, int? expectedLe, int? expectedPs, int? expectedPe, XamlAttribute node)
        {
            Assert.AreEqual(expectedLs, node.LineNumberValueStart, prefix + "Line number value start");
            Assert.AreEqual(expectedLe, node.LineNumberValueEnd, prefix + "Line number value end");
            Assert.AreEqual(expectedPs, node.LinePositionValueStart, prefix + "Position value start");
            Assert.AreEqual(expectedPe, node.LinePositionValueEnd, prefix + "Position value end");
        }

        //         12345678 9012345 6789012     
        [TestCase("<tag a1=\"value1\"/>")]
        public void SingleAttribute(string xamlText)
        {
            XamlParser parser = new XamlParser(Errors);
            XamlMainObjectNode mainNode = parser.ParseFromString(xamlText);
            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(XamlNodeBase.EState.Closed , mainNode.State, "Node state");
            Assert.AreEqual(0, mainNode.Children.Count, "Wrong children count");
            Assert.AreEqual(1, mainNode.Attributes.Count, "Wrong attributes count");
            XamlNodeBase node = mainNode.Attributes[0];
            Assert.AreEqual(XamlNodeBase.ENodeType.Attribute, node.NodeType, "Wrong attribute type");
            XamlAttribute attribute = node as XamlAttribute;
            Assert.AreEqual("a1",attribute.Name,"Wrong attribute name");
            Assert.AreEqual("value1", attribute.Value, "Wrong attribute value");
            CheckTextRange("Attribute:", 1, 1, 6, 16, node);
            CheckTextRangeValue("Attribute value:", 1, 1, 9, 16, attribute);
        }
    }
}
