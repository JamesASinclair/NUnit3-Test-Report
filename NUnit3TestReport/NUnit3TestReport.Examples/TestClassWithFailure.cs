using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture, Category("ExcludeOnBuildServer")]
    public class TestClassWithFailure
    {
        [Test]
        public void FailingTest()
        {
            Console.WriteLine("Test Console Output");
            Assert.That("this is the text", Is.EqualTo("this is not the text"));
        }
    }
}
