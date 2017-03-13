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
        public void GetTestResultData_IfTheFileContentsCanBeParsed_ShouldAssignPropertiesFromXml_AndSetIsValidTrue()
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

        [Test]
        public void GetTestResultData_ShouldAlsoReturnDataAboutTheTestCase()
        {
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<test-run id=""2"" testcasecount=""9"" result=""Failed"" total=""1"" passed=""0"" failed=""1"" inconclusive=""0"" skipped=""0"" asserts=""1"" engine-version=""3.6.1.0"" clr-version=""4.0.30319.42000"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.653117"">
  <test-suite type=""Assembly"" id=""0-1016"" name=""NUnit3TestReport.Examples.dll"" fullname=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\NUnit3TestReport.Examples.dll"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.075602"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
    <failure>
      <message><![CDATA[One or more child tests had errors]]></message>
    </failure>
    <test-suite type=""TestSuite"" id=""0-1017"" name=""NUnit3TestReport"" fullname=""NUnit3TestReport"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.056057"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
      <failure>
        <message><![CDATA[One or more child tests had errors]]></message>
      </failure>
      <test-suite type=""TestSuite"" id=""0-1018"" name=""Examples"" fullname=""NUnit3TestReport.Examples"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.052165"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
        <failure>
          <message><![CDATA[One or more child tests had errors]]></message>
        </failure>
        <test-suite type=""TestFixture"" id=""0-1005"" name=""TestClassWithFailure"" fullname=""NUnit3TestReport.Examples.TestClassWithFailure"" classname=""NUnit3TestReport.Examples.TestClassWithFailure"" runstate=""Runnable"" testcasecount=""1"" result=""Failed"" site=""Child"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.046262"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
          <properties>
            <property name=""Category"" value=""ExcludeOnBuildServer"" />
          </properties>
          <failure>
            <message><![CDATA[One or more child tests had errors]]></message>
          </failure>
          <test-case id=""0-1006"" name=""FailingTest"" fullname=""NUnit3TestReport.Examples.TestClassWithFailure.FailingTest"" methodname=""FailingTest"" classname=""NUnit3TestReport.Examples.TestClassWithFailure"" runstate=""Runnable"" seed=""572200901"" result=""Failed"" start-time=""2017-03-13 20:22:28Z"" end-time=""2017-03-13 20:22:28Z"" duration=""0.034799"" asserts=""1"">
            <failure>
              <message><![CDATA[  Expected string length 20 but was 16. Strings differ at index 8.
  Expected: ""this is not the text""
  But was:  ""this is the text""
  -------------------^
]]></message>
              <stack-trace><![CDATA[at NUnit3TestReport.Examples.TestClassWithFailure.FailingTest() in C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\TestClassWithFailure.cs:line 17
]]></stack-trace>
            </failure>
            <output><![CDATA[Test Console Output
]]></output>
            <assertions>
              <assertion result=""Failed"">
                <message><![CDATA[  Expected string length 20 but was 16. Strings differ at index 8.
  Expected: ""this is not the text""
  But was:  ""this is the text""
  -------------------^
]]></message>
                <stack-trace><![CDATA[at NUnit3TestReport.Examples.TestClassWithFailure.FailingTest() in C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\TestClassWithFailure.cs:line 17
]]></stack-trace>
              </assertion>
            </assertions>
          </test-case>
        </test-suite>
      </test-suite>
    </test-suite>
  </test-suite>
</test-run>";

            // Act
            var result = Program.GetTestResultData("validfile.txt", xml);

            // Assert
            Assert.That(result.TestCases.Count, Is.EqualTo(1));
            Assert.That(result.TestCases[0].FullName, Is.EqualTo("NUnit3TestReport.Examples.TestClassWithFailure.FailingTest"));
        }
    }
}