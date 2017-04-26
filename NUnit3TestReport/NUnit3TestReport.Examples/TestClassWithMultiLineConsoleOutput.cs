using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithMultiLineConsoleOutput
    {
        [Test]
        public void TestMultiLineOutput()
        {
            Console.WriteLine(@"<Xml>
    <Tag>This should be shown as xml</Tag>
</Xml>");
        }
    }
}