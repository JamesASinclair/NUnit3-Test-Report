using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Examples
{
    [TestFixture]
    public class TestClassWithMultiLineConsoleOutput
    {
        [Test]
        public void TestMultiLineOutput()
        {
            Console.WriteLine(@"Backup Database 'DirectaTrunk' to 'DirectaTrunk-IntegrationTest.bak'
Config: {""NumberOfRequestTypes"":50,""NumberOfBundles"":50,""NumberOfServices"":50,""NumberOfProducts"":1000}
First get took (ms): 40
Subsquent gets took (for 100000): 173 ms
Subsquent gets took on average (for 100000): 0 ms
Restore Database 'DirectaTrunk' from 'DirectaTrunk-IntegrationTest.bak'
Begin Restore Database attempt:'1'
End Restore Database attempt:'1' Taken:'3672ms'");
        }
    }
}