using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithFailureInSetup
    {
        [SetUp]
        public void SetUp()
        {
            throw new ArgumentException("Setup Failed");
        }

        [Test]
        public void Test()
        {
            Assert.Pass();
        }
    }
}