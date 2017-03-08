using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithInconclusive
    {
        [Test]
        public void TestInconclusive()
        {
            Assert.Inconclusive();
        }
    }
}