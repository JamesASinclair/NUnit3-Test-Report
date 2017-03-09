using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture, Category("ExcludeOnBuildServer")]
    public class TestClassWithInconclusive
    {
        [Test]
        public void TestInconclusive()
        {
            Assert.Inconclusive();
        }
    }
}