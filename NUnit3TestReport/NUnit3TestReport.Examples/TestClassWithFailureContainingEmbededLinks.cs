using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture, Category("ExcludeOnBuildServer")]
    public class TestClassWithFailureContainingEmbededLinks
    {
        [Test]
        public void Test()
        {
            Console.WriteLine(@"<test-report-link>.\Links\LinkedImage.png</test-report-link>");
            Console.WriteLine(@"<test-report-link>.\Links\LinkedHtml.html</test-report-link>");
            Assert.That("if you use the <test-report-link> tag", Is.EqualTo("Links can be made in console output"));
        }
    }
}