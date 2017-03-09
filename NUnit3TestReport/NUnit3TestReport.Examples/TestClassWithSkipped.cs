using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture, Category("ExcludeOnBuildServer")]
    public class TestClassWithSkipped
    {
        [Test, Ignore("this test should be skipped")]
        public void Test()
        {
            Assert.Fail();
        }
    }
}