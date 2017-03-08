using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithSkipped
    {
        [Test, Ignore("this test should be skipped")]
        public void Test()
        {
            Assert.Fail();
        }
    }
}