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

        #region GetProperties

        [Test]
        public void GetProperties_ShouldHandleNull()
        {
            // Arrange
            string[] files = null;

            // Act
            var result = Program.GetProperties(files);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetProperties_ShouldAssign_Properties_FromXml()
        {
            // Arrange
            string[] files = { $@"{TestContext.CurrentContext.TestDirectory}\Example.test.xml" };

            // Act
            var result = Program.GetProperties(files);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Assembly, Is.EqualTo("Example.Test.Dll"));
            Assert.That(result[0].Result, Is.EqualTo("Passed"));
            Assert.That(result[0].Total, Is.EqualTo(28));
            Assert.That(result[0].Passed, Is.EqualTo(25));
            Assert.That(result[0].Failed, Is.EqualTo(0));
            Assert.That(result[0].Inconclusive, Is.EqualTo(1));
            Assert.That(result[0].Skipped, Is.EqualTo(2));
            Assert.That(result[0].Duration, Is.EqualTo(2.171041m));
        }

        [Test]
        public void GetProperties_ShouldHandleMultipleFiles()
        {
            // Next up!
        }

        #endregion

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
