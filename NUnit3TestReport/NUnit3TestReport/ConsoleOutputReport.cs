using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NUnit3TestReport
{
    public class ConsoleOutputReport : IReportBuilder
    {
        public string Build(List<TestRun> testRuns)
        {
            var tableRowsHtml = new StringBuilder();
            foreach (var testRun in testRuns)
            {
                tableRowsHtml.AppendLine(ToHtml(testRun));
            }

            var template = Program.GetEmbeddedResource("NUnit3TestReport.ConsoleOutputTemplate.html");
            var output = template.Replace("##TestResults##", tableRowsHtml.ToString());
            return output.Replace("##FileCount##", testRuns.Count.ToString());
        }

        public string ToHtml(TestRun testRun)
        {
            if (testRun.IsValid)
            {
                var html = new StringBuilder();
                foreach (var testCase in testRun.TestCases.Where(tc => string.IsNullOrWhiteSpace(tc.Console) == false))
                {
                    html.AppendLine(ToHtml(testCase));
                }
                return html.ToString();
            }
            else
            {
                return $"<tr><td>{testRun.FileName}</td><th class='text-danger text-bold'>File could not be parsed</th></tr>";
            }
        }

        public string ToHtml(TestCase testCase)
        {
            if (string.IsNullOrWhiteSpace(testCase.Console))
                return string.Empty;
            else
                return $"<tr><td>{testCase.FullName}</td><td><pre>{testCase.Console}</pre></td></tr>";
        }
    }
}