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
    public class ProgramTests
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
        public void IfExe_NotSuppliedWith2Args_PrintsUsageInfo_And_ExitsWithCode1()
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
            var exitcode = CreateProcess(ExePath, $"TestFiles {OutputFile}", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(0), output);
            Assert.That(File.Exists(OutputFile), Is.True);
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
