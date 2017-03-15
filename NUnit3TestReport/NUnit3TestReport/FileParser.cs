using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

namespace NUnit3TestReport
{
    public class FileParser
    {
        public List<TestRun> Parse(string filePattern)
        {
            var files = GetFiles(filePattern);

            var testRuns = new List<TestRun>();
            foreach (var file in files)
            {
                testRuns.Add(ParseTestRun(Path.GetFileName(file), File.ReadAllText(file)));
            }

            return testRuns;
        }

        private static string[] GetFiles(string filePattern)
        {
            var folder = Path.GetDirectoryName(filePattern);
            var searchPattern = Path.GetFileName(filePattern);
            return Directory.GetFiles(folder, searchPattern);
        }

        public TestRun ParseTestRun(string filename, string fileContents)
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
                return new TestRun() { IsValid = false, FileName = filename };
            }
        }

        private Regex LinkRegex = new Regex(@"<test-report-link>(.*?)</test-report-link>");

        private void ParseTestCases(TestRun testResult, XElement doc)
        {
            foreach (var testCase in doc.Descendants("test-case"))
            {
                var console = testCase.Element("output")?.Value ?? string.Empty;
                var links = LinkRegex.Matches(console).Cast<Match>().Select(m => m.Groups[1].Value).ToList();
                testResult.TestCases.Add(new TestCase()
                {
                    FullName = testCase.Attribute("fullname").Value,
                    Result = testCase.Attribute("result").Value,
                    Duration = double.Parse(testCase.Attribute("duration").Value),
                    FailureMessage = testCase.Element("failure")?.Element("message")?.Value ?? string.Empty,
                    StackTrace = testCase.Element("failure")?.Element("stack-trace")?.Value ?? string.Empty,
                    Console = console,
                    Links = links,
                });
            }
        }
    }
}