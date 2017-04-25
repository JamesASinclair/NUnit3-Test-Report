using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit3TestReport.Tests
{
    [TestFixture]
    public class ExeTests
    {
        public string ExePath => Path.Combine(TestContext.CurrentContext.TestDirectory, "NUnit3TestReport.exe");
        public string OutputFile => Path.Combine(TestContext.CurrentContext.TestDirectory, "output.html");

        [SetUp]
        public void Setup()
        {
            if (File.Exists(OutputFile))
            {
                File.Delete(OutputFile);
            }
        }

        [TestCase("-?")]
        [TestCase("-h")]
        [TestCase("-help")]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode0_IfSuppliedWithHelpOption(string helpOption)
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $"{helpOption}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0));
            Assert.That(output, Does.Contain("Usage:"));
        }

        [Test]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode1_IfNotSuppliedWith_MinusFOption()
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, "", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.Contain("Usage:"));
            Assert.That(output, Does.Contain("Missing required option -f."));
        }

        [Test]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode1_IfArgumentNotSuppliedFor_MinusF()
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, "-f", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.Contain("Usage:"));
            Assert.That(output, Does.Contain("Missing required value for option '-f'."));
        }

        [Test]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode1_IfNotSuppliedWith_MinusOOption()
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\TestFile1.txt", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.Contain("Usage:"));
            Assert.That(output, Does.Contain("Missing required option -o."));
        }

        [Test]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode1_IfArgumentNotSuppliedFor_MinusO()
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\TestFile1.txt -o", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.Contain("Usage:"));
            Assert.That(output, Does.Contain("Missing required value for option '-o'"));
        }

        [TestCase("")]
        [TestCase("-testreport")]
        [TestCase("-t")]
        public void Exe_WillProduceTheTestReport_ByDefault_OrWithWithTestReportOption(string reportOption)
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\TestFile1.txt -o {OutputFile} {reportOption}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("<title>NUnit 3 Test Report</title>"));
        }

        [TestCase("-consolereport")]
        [TestCase("-c")]
        public void Exe_WillProduceTheConsoleReport_IfConsoleReportSpecified(string reportOption)
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\TestFile1.txt -o {OutputFile} {reportOption}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("<title>NUnit 3 Console Output Report</title>"));
        }

        [Test]
        public void Exe_ShouldProduceAFile_WithTheNameOfTheSecondArg()
        {
            // Arrange
            Assert.That(File.Exists(OutputFile), Is.False);

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\TestFiles -o {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.Exists(OutputFile), Is.True);
        }

        [Test]
        public void Exe_ShouldProcess_ASingleInputFile()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\TestFile1.txt -o {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("1 file(s) processed"));
        }

        [Test]
        public void Exe_ShouldProcess_ADirectory()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\* -o {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("3 file(s) processed"));
        }

        [Test]
        public void Exe_ShouldProcess_AFilePattern()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FilePatternTests\*.txt -o {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("2 file(s) processed"));
        }

        [Test]
        public void Exe_IfTheFileCannotBeParsed_TheTestResultFileShouldSaySo()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@"-f .\FileContentTests\EmptyFile.xml -o {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("File could not be parsed"));
        }

        public static int CreateProcess(string filename, string arguments, out string output)
        {
            Process process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = TestContext.CurrentContext.TestDirectory,
                    FileName = filename,
                    Arguments = arguments
                }
            };

            StringBuilder sb = new StringBuilder();
            process.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            output = sb + error;
            return process.ExitCode;
        }
    }
}
