using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Tests
{
    [TestFixture]
    public class GetTestResultDataTests
    {
        [Test]
        public void GetTestResultData_IfTheFileContentsCannotBeParse_TheTestResultsShouldBeInvlaid()
        {
            // Arrange/Act
            var result = Program.GetTestResultData("invalidfile.txt", null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void GetTestResultData_IfTheFileContentsCanBeParsed_ShouldAssignPropertiesFromXml_AndSetIsValueTrue()
        {
            // Arrange
            string file = @"<?xml version='1.0' encoding='utf-8' standalone='no'?>
<test-run id='2' testcasecount='28' result='Passed' total='28' passed='25' failed='0' inconclusive='1' skipped='2' asserts='467' engine-version='3.6.0.0' clr-version='4.0.30319.42000' start-time='2017-03-02 16:45:23Z' end-time='2017-03-02 16:45:25Z' duration='2.171041'>
  <test-suite type='Assembly' id='0-1033' name='Example.Test.Dll' fullname='C:\src\Example.Test\bin\Release\Example.Test.Dll' runstate='Runnable' testcasecount='28' result='Passed' start-time='2017-03-02 16:45:23Z' end-time='2017-03-02 16:45:25Z' duration='1.363629' total='28' passed='28' failed='0' warnings='0' inconclusive='0' skipped='0' asserts='467'>
    <environment framework-version='3.6.0.0' clr-version='4.0.30319.42000' os-version='Microsoft Windows NT 10.0.14393.0' platform='Win32NT' culture='en-GB' uiculture='en-GB' os-architecture='x64' />
    <settings>
      <!-- Elements removed as not required -->
    </settings>
    <properties>
      <!-- Elements removed as not required -->
    </properties>
    <test-suite type='TestSuite' id='0-1034' name='ExampleTestSuiteName' fullname='ExampleTestSuiteName' runstate='Runnable' testcasecount='28' result='Passed' start-time='2017-03-02 16:45:23Z' end-time='2017-03-02 16:45:25Z' duration='1.343287' total='28' passed='28' failed='0' warnings='0' inconclusive='0' skipped='0' asserts='467'>
      <!-- Elements removed as not required -->
    </test-suite>
  </test-suite>
</test-run>
";

            // Act
            var result = Program.GetTestResultData("validfile.txt", file);

            // Assert
            Assert.That(result.FileName, Is.EqualTo("validfile.txt"));
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Result, Is.EqualTo("Passed"));
            Assert.That(result.Total, Is.EqualTo(28));
            Assert.That(result.Passed, Is.EqualTo(25));
            Assert.That(result.Failed, Is.EqualTo(0));
            Assert.That(result.Inconclusive, Is.EqualTo(1));
            Assert.That(result.Skipped, Is.EqualTo(2));
            Assert.That(result.Duration, Is.EqualTo(2.171041m));
        }
    }
}