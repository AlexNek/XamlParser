using System;
using System.Xaml;

using NUnit.Framework;
using Parser;
using Parser.Nodes;

namespace UnitTestProjectXamlReader
{
    [TestFixture]
    public class UnitTestSingleTag : UnitTestParserBase
    {
        [TestCase]
        public void TestNull()
        {
            XamlParser parser = new XamlParser(Errors);
            Assert.Throws(typeof(ArgumentNullException), () => parser.ParseFromString(null));
        }

        [TestCase]
        public void TestEmpty()
        {
            XamlParser parser = new XamlParser(Errors);
            XamlMainObjectNode mainNode = parser.ParseFromString("");
            Assert.IsNull(mainNode, "It must be no parse result");
        }

        [TestCase]
        public void TestSomething()
        {
            XamlParser parser = new XamlParser(Errors);
            Assert.Throws(typeof(XamlParseException), () => parser.ParseFromString("abc"));
        }

        [TestCase]
        public void NotClosedTag()
        {
            XamlParser parser = new XamlParser(Errors);
            XamlMainObjectNode mainNode = parser.ParseFromString("<tag>");
            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(XamlNodeBase.EState.EndTagPresent, mainNode.State, "Node state");
        }

        
        [TestCase("<tag/>")]
        [TestCase("< tag />")]
        [TestCase("<tag />")]
        [TestCase("<tag\r\n/>")]
        [TestCase("<tag  \r\n  />")]
        public void TagClosedInside(string xamlText)
        {
            XamlParser parser = new XamlParser(Errors);
            XamlMainObjectNode mainNode = parser.ParseFromString(xamlText);
            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(XamlNodeBase.EState.Closed, mainNode.State, "Node state");
        }

        [TestCase("<tag></tag>", XamlNodeBase.EState.Closed | XamlNodeBase.EState.EndTagPresent)]
        [TestCase("<tag>  \r\n</tag>", XamlNodeBase.EState.Closed | XamlNodeBase.EState.EndTagPresent)]
        public void TagClosed(string xamlText, XamlNodeBase.EState tagState)
        {
            XamlParser parser = new XamlParser(Errors);
            XamlMainObjectNode mainNode = parser.ParseFromString(xamlText);
            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(tagState, mainNode.State, "Node state");
        }

        [TestCase("<tag>123</tag>", 1, 14,1, 6,1,8)]
        [TestCase("<tag>\r\n123\r\n</tag>", 3, 6, 2,1,2,3)]
        public void TagWithValue(string xamlText, int endLineTag, int endPosTag, int startLineText, int startPosText,int endLineText, int endPosText)
        {
            XamlParser parser = new XamlParser(Errors);
            //                                                         12345678901234  
            XamlMainObjectNode mainNode = parser.ParseFromString(xamlText);

            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual(
                XamlNodeBase.EState.Closed | XamlNodeBase.EState.EndTagPresent | XamlNodeBase.EState.TextNodePresent,
                mainNode.State,
                "Node state");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(0, mainNode.Attributes.Count, "Wrong number of attributes");
            Assert.AreEqual(1, mainNode.Children.Count, "Wrong number of children");
            CheckTextRange("Tag:", 1, endLineTag, 1, endPosTag, mainNode);

            XamlTextNode textNode = mainNode.Children[0] as XamlTextNode;
            Assert.IsNotNull(textNode, "It must be text node as first child");
            Assert.AreEqual("123", textNode.Value, "Not expected text value");
            CheckTextRange("Text:", startLineText, endLineText, startPosText, endPosText, textNode);
        }
    }
}
