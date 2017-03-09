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
            Assert.Fail();
        }
    }
}
