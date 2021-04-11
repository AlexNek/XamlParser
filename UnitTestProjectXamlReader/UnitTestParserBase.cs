using NUnit.Framework;

using Parser;
using Parser.Nodes;

namespace UnitTestProjectXamlReader
{
    public class UnitTestParserBase
    {
        private readonly ExceptionErrorHandler _errors = new ExceptionErrorHandler();

        protected static void CheckTextRange(string prefix, int? expectedLs, int? expectedLe, int? expectedPs, int? expectedPe, XamlNodeBase node)
        {
            Assert.AreEqual(expectedLs, node.LineNumberStart, prefix + "Line number start");
            Assert.AreEqual(expectedLe, node.LineNumberEnd, prefix + "Line number end");
            Assert.AreEqual(expectedPs, node.LinePositionStart, prefix + "Position start");
            Assert.AreEqual(expectedPe, node.LinePositionEnd, prefix + "Position end");
        }

        protected ExceptionErrorHandler Errors
        {
            get
            {
                return _errors;
            }
        }
    }
}
