using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithPasses
    {
        [Test]
        public void Test()
        {
            Assert.Pass();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}