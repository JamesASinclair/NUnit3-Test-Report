﻿using System;
using System.Linq;
using NUnit.Framework;

namespace NUnit3TestReport.Tests
{
    [TestFixture]
    public class FileParserTests
    {
        [Test]
        public void ParseTestRun_IfTheFileContentsCannotBeParsed_TheTestRunShouldBeInvlaid()
        {
            // Arrange/Act
            var result = new FileParser().ParseTestRun("invalidfile.txt", null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ParseTestRun_IfTheFileContentsCanBeParsed_PropertiesShouldBeAssigned()
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
            var result = new FileParser().ParseTestRun("validfile.txt", file);

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
        public void ParseTestRun_ShouldReturnDataAboutAFailedTestCase()
        {
            #region xml
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
            #endregion

            // Act
            var result = new FileParser().ParseTestRun("validfile.txt", xml);

            // Assert
            Assert.That(result.IsValid);
            Assert.That(result.TestCases.Count, Is.EqualTo(1));
            Assert.That(result.TestCases[0].FullName, Is.EqualTo("NUnit3TestReport.Examples.TestClassWithFailure.FailingTest"));
            Assert.That(result.TestCases[0].Result, Is.EqualTo("Failed"));
            Assert.That(result.TestCases[0].Duration, Is.EqualTo(0.034799d));
            Assert.That(result.TestCases[0].FailureMessage, Is.EqualTo(@"  Expected string length 20 but was 16. Strings differ at index 8.
  Expected: ""this is not the text""
  But was:  ""this is the text""
  -------------------^
"));
            Assert.That(result.TestCases[0].StackTrace, Is.EqualTo(@"at NUnit3TestReport.Examples.TestClassWithFailure.FailingTest() in C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\TestClassWithFailure.cs:line 17
"));
            Assert.That(result.TestCases[0].Console, Is.EqualTo(@"Test Console Output
"));
        }

        [Test]
        public void ParseTestRun_ShouldReturnDataAboutAPassedTestCase()
        {
            #region xml
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<test-run id=""2"" testcasecount=""8"" result=""Passed"" total=""1"" passed=""1"" failed=""0"" inconclusive=""0"" skipped=""0"" asserts=""0"" engine-version=""3.6.1.0"" clr-version=""4.0.30319.42000"" start-time=""2017-03-13 21:09:24Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.628128"">
  <command-line><![CDATA[""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\..\..\..\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe""  C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\NUnit3TestReport.Examples.dll --framework=net-4.5 --where ""class=NUnit3TestReport.Examples.TestClassWithPasses"" --result:C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\OnePass.test.xml]]></command-line>
  <filter>
    <class>NUnit3TestReport.Examples.TestClassWithPasses</class>
  </filter>
  <test-suite type=""Assembly"" id=""0-1015"" name=""NUnit3TestReport.Examples.dll"" fullname=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\NUnit3TestReport.Examples.dll"" runstate=""Runnable"" testcasecount=""8"" result=""Passed"" start-time=""2017-03-13 21:09:25Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.059607"" total=""1"" passed=""1"" failed=""0"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""0"">
    <environment framework-version=""3.6.1.0"" clr-version=""4.0.30319.42000"" os-version=""Microsoft Windows NT 10.0.14393.0"" platform=""Win32NT"" cwd=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug"" machine-name=""BIOMMAT"" user=""mat.roberts"" user-domain=""BIOMNI-UK"" culture=""en-GB"" uiculture=""en-GB"" os-architecture=""x64"" />
    <settings>
      <setting name=""RuntimeFramework"" value=""net-4.5"" />
      <setting name=""DisposeRunners"" value=""True"" />
      <setting name=""WorkDirectory"" value=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug"" />
      <setting name=""ImageRuntimeVersion"" value=""4.0.30319"" />
      <setting name=""ImageTargetFrameworkName"" value="".NETFramework,Version=v4.6.1"" />
      <setting name=""ImageRequiresX86"" value=""False"" />
      <setting name=""ImageRequiresDefaultAppDomainAssemblyResolver"" value=""False"" />
      <setting name=""NumberOfTestWorkers"" value=""8"" />
    </settings>
    <properties>
      <property name=""_PID"" value=""3824"" />
      <property name=""_APPDOMAIN"" value=""domain-"" />
    </properties>
    <test-suite type=""TestSuite"" id=""0-1016"" name=""NUnit3TestReport"" fullname=""NUnit3TestReport"" runstate=""Runnable"" testcasecount=""8"" result=""Passed"" start-time=""2017-03-13 21:09:25Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.040046"" total=""1"" passed=""1"" failed=""0"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""0"">
      <test-suite type=""TestSuite"" id=""0-1017"" name=""Examples"" fullname=""NUnit3TestReport.Examples"" runstate=""Runnable"" testcasecount=""8"" result=""Passed"" start-time=""2017-03-13 21:09:25Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.036203"" total=""1"" passed=""1"" failed=""0"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""0"">
        <test-suite type=""TestFixture"" id=""0-1011"" name=""TestClassWithPasses"" fullname=""NUnit3TestReport.Examples.TestClassWithPasses"" classname=""NUnit3TestReport.Examples.TestClassWithPasses"" runstate=""Runnable"" testcasecount=""1"" result=""Passed"" start-time=""2017-03-13 21:09:25Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.030126"" total=""1"" passed=""1"" failed=""0"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""0"">
          <properties>
            <property name=""Category"" value=""ExcludeOnBuildServer"" />
          </properties>
          <test-case id=""0-1012"" name=""Test"" fullname=""NUnit3TestReport.Examples.TestClassWithPasses.Test"" methodname=""Test"" classname=""NUnit3TestReport.Examples.TestClassWithPasses"" runstate=""Runnable"" seed=""333093390"" result=""Passed"" start-time=""2017-03-13 21:09:25Z"" end-time=""2017-03-13 21:09:25Z"" duration=""0.019094"" asserts=""0"">
            <reason>
              <message><![CDATA[]]></message>
            </reason>
          </test-case>
        </test-suite>
      </test-suite>
    </test-suite>
  </test-suite>
</test-run>";
            #endregion

            // Act
            var result = new FileParser().ParseTestRun("", xml);

            // Assert
            Assert.That(result.IsValid);
            Assert.That(result.TestCases.Count, Is.EqualTo(1));
            Assert.That(result.TestCases[0].FullName, Is.EqualTo("NUnit3TestReport.Examples.TestClassWithPasses.Test"));
            Assert.That(result.TestCases[0].Result, Is.EqualTo("Passed"));
            Assert.That(result.TestCases[0].Duration, Is.EqualTo(0.019094));
            Assert.That(result.TestCases[0].FailureMessage, Is.EqualTo(string.Empty));
            Assert.That(result.TestCases[0].StackTrace, Is.EqualTo(string.Empty));
            Assert.That(result.TestCases[0].Console, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ParseTestRun_ShouldExtractLinks_FromConsoleOutput()
        {
            #region xml
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<test-run id=""2"" testcasecount=""9"" result=""Failed"" total=""1"" passed=""0"" failed=""1"" inconclusive=""0"" skipped=""0"" asserts=""1"" engine-version=""3.6.1.0"" clr-version=""4.0.30319.42000"" start-time=""2017-03-15 20:28:17Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.684879"">
  <command-line><![CDATA[""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\..\..\..\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe""  C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\NUnit3TestReport.Examples.dll --framework=net-4.5 --where ""class=NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks"" --result:C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\EmbededLinks.test.xml]]></command-line>
  <test-suite type=""Assembly"" id=""0-1017"" name=""NUnit3TestReport.Examples.dll"" fullname=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug\NUnit3TestReport.Examples.dll"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-15 20:28:18Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.081457"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
    <environment framework-version=""3.6.1.0"" clr-version=""4.0.30319.42000"" os-version=""Microsoft Windows NT 10.0.14393.0"" platform=""Win32NT"" cwd=""C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\bin\Debug"" machine-name=""BIOMMAT"" user=""mat.roberts"" user-domain=""BIOMNI-UK"" culture=""en-GB"" uiculture=""en-GB"" os-architecture=""x64"" />
    <failure>
      <message><![CDATA[One or more child tests had errors]]></message>
    </failure>
    <test-suite type=""TestSuite"" id=""0-1018"" name=""NUnit3TestReport"" fullname=""NUnit3TestReport"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-15 20:28:18Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.059964"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
      <failure>
        <message><![CDATA[One or more child tests had errors]]></message>
      </failure>
      <test-suite type=""TestSuite"" id=""0-1019"" name=""Examples"" fullname=""NUnit3TestReport.Examples"" runstate=""Runnable"" testcasecount=""9"" result=""Failed"" site=""Child"" start-time=""2017-03-15 20:28:18Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.056442"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
        <failure>
          <message><![CDATA[One or more child tests had errors]]></message>
        </failure>
        <test-suite type=""TestFixture"" id=""0-1007"" name=""TestClassWithFailureContainingEmbededLinks"" fullname=""NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks"" classname=""NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks"" runstate=""Runnable"" testcasecount=""1"" result=""Failed"" site=""Child"" start-time=""2017-03-15 20:28:18Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.050601"" total=""1"" passed=""0"" failed=""1"" warnings=""0"" inconclusive=""0"" skipped=""0"" asserts=""1"">
          <properties>
            <property name=""Category"" value=""ExcludeOnBuildServer"" />
          </properties>
          <failure>
            <message><![CDATA[One or more child tests had errors]]></message>
          </failure>
          <test-case id=""0-1008"" name=""Test"" fullname=""NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks.Test"" methodname=""Test"" classname=""NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks"" runstate=""Runnable"" seed=""750257848"" result=""Failed"" start-time=""2017-03-15 20:28:18Z"" end-time=""2017-03-15 20:28:18Z"" duration=""0.037905"" asserts=""1"">
            <failure>
              <message><![CDATA[  Expected string length 37 but was 35. Strings differ at index 0.
  Expected: ""if you use the <test-report-link> tag""
  But was:  ""Links can be made in console output""
  -----------^
]]></message>
              <stack-trace><![CDATA[at NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks.Test() in C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\TestClassWithFailureContainingEmbededLinks.cs:line 15
]]></stack-trace>
            </failure>
            <output><![CDATA[<test-report-link>.\Links\LinkedImage.png</test-report-link>
<test-report-link>.\Links\LinkedHtml.html</test-report-link>
]]></output>
            <assertions>
              <assertion result=""Failed"">
                <message><![CDATA[  Expected string length 37 but was 35. Strings differ at index 0.
  Expected: ""if you use the <test-report-link> tag""
  But was:  ""Links can be made in console output""
  -----------^
]]></message>
                <stack-trace><![CDATA[at NUnit3TestReport.Examples.TestClassWithFailureContainingEmbededLinks.Test() in C:\github\NUnit3-Test-Report\NUnit3TestReport\NUnit3TestReport.Examples\TestClassWithFailureContainingEmbededLinks.cs:line 15
]]></stack-trace>
              </assertion>
            </assertions>
          </test-case>
        </test-suite>
      </test-suite>
    </test-suite>
  </test-suite>
</test-run>";
            #endregion

            // Act
            var result = new FileParser().ParseTestRun("", xml);

            // Assert
            Assert.That(result.IsValid);
            Assert.That(result.TestCases.Count, Is.EqualTo(1));
            Assert.That(result.TestCases[0].Console, Is.EqualTo(
@"<test-report-link>.\Links\LinkedImage.png</test-report-link>
<test-report-link>.\Links\LinkedHtml.html</test-report-link>
"));
            Assert.That(result.TestCases[0].Links.Count, Is.EqualTo(2));
            Assert.That(result.TestCases[0].Links[0], Is.EqualTo(@".\Links\LinkedImage.png"));
            Assert.That(result.TestCases[0].Links[1], Is.EqualTo(@".\Links\LinkedHtml.html"));
        }
    }
}