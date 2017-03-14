﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
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

            var testRuns = new List<TestRun>();
            foreach (var file in files)
            {
                testRuns.Add(ParseTestRun(Path.GetFileName(file), File.ReadAllText(file)));
            }

            var tableRowsHtml = new StringBuilder();
            foreach (var testRun in testRuns)
            {
                tableRowsHtml.AppendLine(testRun.ToHtml());
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

        public static TestRun ParseTestRun(string filename, string fileContents)
        {
            try
            {
                var doc = XElement.Parse(fileContents);
                var testResult = new TestRun
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
                return new TestRun() {IsValid = false, FileName = filename };
            }
        }

        private static void ParseTestCases(TestRun testResult, XElement doc)
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

    public class TestRun
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
        public IEnumerable<TestCase> FailedTestCases => TestCases.Where(tc => tc.Result.Equals("Failed", StringComparison.OrdinalIgnoreCase));

        public string ToHtml()
        {
            if (IsValid)
            {
                var html = new StringBuilder();
                html.AppendLine($@"<tr>
                    <td>{FileName}</td>
                    <th class='{(Result.Equals("Passed") ? "text-success" : "text-danger text-bold")}'>{Result}</th>
                    <td class='text-right'>{Passed}</td>
                    <td class='text-right'>{Failed}</td>
                    <td class='text-right'>{Inconclusive}</td>
                    <td class='text-right'>{Skipped}</td>
                    <td class='text-right'>{Total}</td>
                    <td class='text-right'>{TimeSpan.FromSeconds(Duration).ToString(@"h\:mm\:ss")}</td>
                </tr>");
                if (FailedTestCases.Any())
                {
                    html.AppendLine("<tr class='noborders'><td colspan='8'>");
                    foreach (var failure in FailedTestCases)
                    {
                        html.AppendLine($"<pre><strong>{failure.FullName}</strong>" +
                                        $"\r\n{HttpUtility.HtmlEncode(failure.FailureMessage)}" +
                                        $"\r\n{HttpUtility.HtmlEncode(failure.StackTrace)}</pre>");

                    }
                    html.AppendLine("</td></tr>");
                }
                return html.ToString();
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
