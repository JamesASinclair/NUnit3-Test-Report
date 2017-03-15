using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NUnit3TestReport
{
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
                html.AppendLine($@"<tr {(FailedTestCases.Any() ? "class='toggle'" : "")}>
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
                    html.AppendLine("<tr><td colspan='8'>");
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
}