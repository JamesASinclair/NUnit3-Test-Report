using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit3TestReport.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        public static int CreateProcess(string filename, string arguments, out string output)
        {
            Process process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = TestContext.CurrentContext.TestDirectory,
                    FileName = filename,
                    Arguments = arguments
                }
            };

            StringBuilder sb = new StringBuilder();
            process.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            output = sb.ToString();
            return process.ExitCode;
        }

        [Test]
        public void IfNotSuppliedWith2Args_ExitsWithCode1()
        {
            // Arrange
            string output = null;
            var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "NUnit3TestReport.exe");

            // Act
            var exitcode = CreateProcess(fileName, "", out output);

            // Assert
            Assert.That(exitcode, Is.EqualTo(1));
            Assert.That(output, Does.StartWith("NUnit3TestReport.exe"));

            Console.WriteLine(output);
        }
    }
}
