using System;
using System.Xaml;

using NUnit.Framework;

using Parser;
using Parser.Nodes;

namespace UnitTestProjectXamlReader
{
    [TestFixture]
    public class UnitTestComments : UnitTestParserBase
    {
        [TestCase]
        public void SingleComment()
        {
            XamlParser parser = new XamlParser(Errors);
            Assert.Throws(typeof(XamlParseException), () => parser.ParseFromString("<!--Comment 1-- >"));
        }

        [TestCase]
        public void CommentBeforeMainNode()
        {
            XamlParser parser = new XamlParser(Errors);
            Assert.Throws(typeof(XamlParseException), () => parser.ParseFromString("<!--Comment 1--><tag/>"));
            
        }

        [TestCase]
        public void CommentAfterMainNode()
        {
            string xamlText = "<tag/><!--Comment 1-->";
            XamlParser parser = new XamlParser(Errors);
            Assert.Throws(typeof(XamlParseException), () => parser.ParseFromString(xamlText));
            
        }

        //         1234567890123456789012     
        [TestCase("<tag><!--Comment 1--></tag>")]
        [TestCase("<tag><!--Comment 1--><tag2/></tag>")]
        [TestCase("<tag><!--Comment 1-->\r\n<tag2/></tag>")]
        public void CommentInsideMainNode(string xamlText)
        {
            XamlParser parser = new XamlParser(Errors);
            
            XamlMainObjectNode mainNode = parser.ParseFromString(xamlText);
            Assert.IsNotNull(mainNode, "It must be parse result");
            Assert.AreEqual("tag", mainNode.Name, "Wrong tag name");
            Assert.AreEqual(XamlNodeBase.EState.Closed | XamlNodeBase.EState.EndTagPresent, mainNode.State, "Node state");
            Assert.AreEqual(1, mainNode.Children.Count, "Wrong children count");
            XamlNodeBase node = mainNode.Children[0];
            Assert.AreEqual(XamlNodeBase.ENodeType.Comment, node.NodeType, "Wrong node type");
            XamlCommentNode comment = node as XamlCommentNode;
            Assert.AreEqual("Comment 1", comment.Comment,"Comment text");
            CheckTextRange("Comment:", 1, 1, 6, 21, comment);
        }
    }
}
