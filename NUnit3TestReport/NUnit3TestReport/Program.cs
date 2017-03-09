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
                var xElement = XElement.Parse(fileContents);
                return new TestResultData
                {
                    IsValid = true,
                    FileName = filename,
                    Result = xElement.Element("test-suite").Attribute("result").Value,
                    Total = int.Parse(!string.IsNullOrEmpty(xElement.Attribute("total").Value) ? xElement.Attribute("total").Value : "0"),
                    Passed = int.Parse(!string.IsNullOrEmpty(xElement.Attribute("passed").Value) ? xElement.Attribute("passed").Value : "0"),
                    Failed = int.Parse(!string.IsNullOrEmpty(xElement.Attribute("failed").Value) ? xElement.Attribute("failed").Value : "0"),
                    Inconclusive = int.Parse(!string.IsNullOrEmpty(xElement.Attribute("inconclusive").Value) ? xElement.Attribute("inconclusive").Value : "0"),
                    Skipped = int.Parse(!string.IsNullOrEmpty(xElement.Attribute("skipped").Value) ? xElement.Attribute("skipped").Value : "0"),
                    Duration = decimal.Parse(!string.IsNullOrEmpty(xElement.Attribute("duration").Value) ? xElement.Attribute("duration").Value : "0")
                };
            }
            catch
            {
                return new TestResultData() {IsValid = false, FileName = filename };
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
        public decimal Duration { get; set; }

        public bool HasFailedTests => this.Failed > 0;

        public string ToHtml()
        {
            if (IsValid)
            {
                return $@"<tr>
                    <th>{FileName}</th>
                    <td>{Result}</td>
                    <td>{Total}</td>
                    <td>{Passed}</td>
                    <td>{Failed}</td>
                    <td>{Inconclusive}</td>
                    <td>{Skipped}</td>
                    <td>{Duration}</td>
                </tr>";
            }
            else
            {
                return $"<tr class='danger'><th>{FileName}</th><td colspan='7'>File could not be parsed</td></tr>";
            }
        }
    }
}
