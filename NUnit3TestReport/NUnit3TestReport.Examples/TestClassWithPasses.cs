using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture, Category("ExcludeOnBuildServer")]
    public class TestClassWithPasses
    {
        [Test]
        public void Test()
        {
            Assert.Pass();
        }
    }
}