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

        [Test]
        public void Exe_ShouldPrintUsageInfo_And_ExitsWithCode1_IfNotSuppliedWith2Args()
        {
            // Arrange

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, "", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.StartWith("NUnit3TestReport.exe"));
        }

        [Test]
        public void Exe_ShouldProduceAFile_WithTheNameOfTheSecondArg()
        {
            // Arrange
            Assert.That(File.Exists(OutputFile), Is.False);

            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@".\TestFiles {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.Exists(OutputFile), Is.True);
        }

        [Test]
        public void Exe_ShouldProcess_ASingleInputFile()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@".\FilePatternTests\TestFile1.txt {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("1 file(s) processed"));
        }

        [Test]
        public void Exe_ShouldProcess_ADirectory()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@".\FilePatternTests\* {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("3 file(s) processed"));
        }

        [Test]
        public void Exe_ShouldProcess_AFilePattern()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@".\FilePatternTests\*.txt {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("2 file(s) processed"));
        }

        [Test]
        public void Exe_IfTheFileCannotBeParsed_TheTestResultFileShouldSaySo()
        {
            // Act
            string output = null;
            var exitcode = CreateProcess(ExePath, $@".\FileContentTests\EmptyFile.xml {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.ReadAllText(OutputFile), Does.Contain("<tr class='danger'><th>EmptyFile.xml</th><td colspan='7'>File could not be parsed</td></tr>"));
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
