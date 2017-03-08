using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestCaseWithSomePassesAndFailes
    {
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 2)]
        public void Test(int a, int b)
        {
            Assert.That(a, Is.EqualTo(b));
        }
    }
}