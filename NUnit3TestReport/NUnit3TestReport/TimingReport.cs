using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit3TestReport
{
    public class TimingReport : IReportBuilder
    {
        public string Build(List<TestRun> testRuns)
        {
            var tableRowsHtml = new StringBuilder();
            tableRowsHtml.AppendLine(ToHtml(testRuns));

            var template = Program.GetEmbeddedResource("NUnit3TestReport.TimingReportTemplate.html");
            var output = template.Replace("##TestResults##", tableRowsHtml.ToString());
            return output.Replace("##FileCount##", testRuns.Count.ToString());
        }

        public string ToHtml(List<TestRun> testRuns)
        {
            var testCases = testRuns.SelectMany(t => t.TestCases);
            
            var html = new StringBuilder();
            foreach (var testCase in testCases.OrderByDescending(t => t.Duration))
            {
                html.AppendLine($"<tr><td>{testCase.FullName}</td><td>{TimeSpan.FromSeconds(testCase.Duration).ToString(@"c")}</td></tr>");
            }
            return html.ToString();
        }
    }
}