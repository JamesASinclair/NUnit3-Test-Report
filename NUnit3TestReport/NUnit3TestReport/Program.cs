using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace NUnit3TestReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            var files = GetFiles(args[0]);

            var testResults = new List<TestResultData>();
            foreach (var file in files)
            {
                testResults.Add(GetTestResultData(Path.GetFileName(file), File.ReadAllText(file)));
            }

            var tableRowsHtml = new StringBuilder();
            foreach (var testResult in testResults)
            {
                tableRowsHtml.AppendLine(testResult.ToHtml());
            }

            var template = GetEmbeddedResource("NUnit3TestReport.Template.html");
            var output = template.Replace("##TestResults##", tableRowsHtml.ToString());
            output = output.Replace("##FileCount##", files.Length.ToString());
            File.WriteAllText(args[1], output);
        }

        private static void PrintUsage()
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($@"{info.InternalName}
Version {info.ProductVersion}
{info.LegalCopyright}

{info.Comments}

Usage:
    {info.InternalName} [input-path-with-wildcards] [output-path]
Examples:
    Single input file: {info.InternalName} TestResults.xml TestResult.html
    Folder:            {info.InternalName} C:\Folder\* TestResult.html
    File pattern:      {info.InternalName} C:\Folder\*.xml TestResult.html");
        }

        public static string[] GetFiles(string filePattern)
        {
            var folder = Path.GetDirectoryName(filePattern);
            var searchPattern = Path.GetFileName(filePattern);
            return Directory.GetFiles(folder, searchPattern);
        }

        public static string GetEmbeddedResource(string fullyQualifiedName)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedName)))
            {
                return reader.ReadToEnd();
            }
        }

        public static TestResultData GetTestResultData(string filename, string fileContents)
        {
            try
            {
                var doc = XElement.Parse(fileContents);
                var testResult = new TestResultData
                {
                    IsValid = true,
                    FileName = filename,
                    Result = doc.Attribute("result").Value,
                    Total = int.Parse(doc.Attribute("total").Value),
                    Passed = int.Parse(doc.Attribute("passed").Value),
                    Failed = int.Parse(doc.Attribute("failed").Value),
                    Inconclusive = int.Parse(doc.Attribute("inconclusive").Value),
                    Skipped = int.Parse(doc.Attribute("skipped").Value),
                    Duration = double.Parse(doc.Attribute("duration").Value),
                };
                ParseTestCases(testResult, doc);
                return testResult;
            }
            catch
            {
                return new TestResultData() {IsValid = false, FileName = filename };
            }
        }

        private static void ParseTestCases(TestResultData testResult, XElement doc)
        {
            foreach (var testCase in doc.Descendants("test-case"))
            {
                testResult.TestCases.Add(new TestCase()
                {
                    FullName = testCase.Attribute("fullname").Value,
                    Result = testCase.Attribute("result").Value,
                    Duration = double.Parse(testCase.Attribute("duration").Value),
                    FailureMessage = testCase.Element("failure")?.Element("message")?.Value ?? string.Empty,
                    StackTrace = testCase.Element("failure")?.Element("stack-trace")?.Value ?? string.Empty,
                    Console = testCase.Element("output")?.Value ?? string.Empty,
                });
            }
        }
    }

    public class TestResultData
    {
        public bool IsValid { get; set; }
        public string FileName { get; set; }
        public string Result { get; set; }
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Inconclusive { get; set; }
        public int Skipped { get; set; }
        public double Duration { get; set; }
        public List<TestCase> TestCases { get; } = new List<TestCase>();

        public string ToHtml()
        {
            if (IsValid)
            {
                return $@"<tr>
                    <td>{FileName}</td>
                    <th class='{(Result.Equals("Passed") ? "text-success" : "text-danger text-bold")}'>{Result}</th>
                    <td class='text-right'>{Passed}</td>
                    <td class='text-right'>{Failed}</td>
                    <td class='text-right'>{Inconclusive}</td>
                    <td class='text-right'>{Skipped}</td>
                    <td class='text-right'>{Total}</td>
                    <td class='text-right'>{TimeSpan.FromSeconds(Duration).ToString(@"h\:mm\:ss")}</td>
                </tr>";
            }
            else
            {
                return $"<tr><td>{FileName}</td><th colspan='7' class='text-danger text-bold'>File could not be parsed</th></tr>";
            }
        }
    }

    public class TestCase
    {
        public string FullName { get; set; }
        public string Result { get; set; }
        public double Duration { get; set; }
        public string FailureMessage { get; set; }
        public string StackTrace { get; set; }
        public string Console { get; set; }
    }
}
