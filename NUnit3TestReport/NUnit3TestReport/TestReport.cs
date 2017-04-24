using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace NUnit3TestReport
{
    public class TestReport : IReportBuilder
    {
        public string Build(List<TestRun> testRuns)
        {
            var tableRowsHtml = new StringBuilder();
            foreach (var testRun in testRuns)
            {
                tableRowsHtml.AppendLine(ToHtml(testRun));
            }

            var template = Program.GetEmbeddedResource("NUnit3TestReport.TestReportTemplate.html");
            var output = template.Replace("##TestResults##", tableRowsHtml.ToString());
            return output.Replace("##FileCount##", testRuns.Count.ToString());
        }

        public string ToHtml(TestRun testRun)
        {
            if (testRun.IsValid)
            {
                var html = new StringBuilder();
                html.AppendLine($@"<tr {(testRun.FailedTestCases.Any() ? "class='toggle'" : "")}>
                    <td>{testRun.FileName}</td>
                    <th class='{(testRun.Result.Equals("Passed") ? "text-success" : "text-danger text-bold")}'>{testRun.Result}</th>
                    <td class='text-right'>{testRun.Passed}</td>
                    <td class='text-right'>{testRun.Failed}</td>
                    <td class='text-right'>{testRun.Inconclusive}</td>
                    <td class='text-right'>{testRun.Skipped}</td>
                    <td class='text-right'>{testRun.Total}</td>
                    <td class='text-right'>{TimeSpan.FromSeconds(testRun.Duration).ToString(@"h\:mm\:ss")}</td>
                </tr>");
                if (testRun.FailedTestCases.Any())
                {
                    html.AppendLine("<tr><td colspan='8'>");
                    foreach (var failure in testRun.FailedTestCases)
                    {
                        html.AppendLine(ToHtml(failure));

                    }
                    html.AppendLine("</td></tr>");
                }
                return html.ToString();
            }
            else
            {
                return $"<tr><td>{testRun.FileName}</td><th colspan='7' class='text-danger text-bold'>File could not be parsed</th></tr>";
            }
        }

        public string ToHtml(TestCase testCase)
        {
            var html = new StringBuilder();
            html.AppendLine($"<pre><strong>{testCase.FullName}</strong>");
            html.AppendLine($"{HttpUtility.HtmlEncode(testCase.FailureMessage)}");
            html.AppendLine($"{HttpUtility.HtmlEncode(testCase.StackTrace)}");
            foreach (var link in testCase.Links)
            {
                html.AppendLine($"<a href='{link}' target='_blank'>{link}</a>");
            }
            html.Append("</pre>");
            return html.ToString();
        }
    }
}