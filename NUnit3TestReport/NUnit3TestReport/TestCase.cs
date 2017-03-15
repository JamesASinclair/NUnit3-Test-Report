using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NUnit3TestReport
{
    public class TestCase
    {
        public string FullName { get; set; }
        public string Result { get; set; }
        public double Duration { get; set; }
        public string FailureMessage { get; set; }
        public string StackTrace { get; set; }
        public string Console { get; set; }
        public List<string> Links { get; set; } = new List<string>();

        public string ToHtml()
        {
            var html = new StringBuilder();
            html.AppendLine($"<pre><strong>{FullName}</strong>");
            html.AppendLine($"{HttpUtility.HtmlEncode(FailureMessage)}");
            html.AppendLine($"{HttpUtility.HtmlEncode(StackTrace)}");
            foreach (var link in Links)
            {
                html.AppendLine($"<a href='{link}' target='_blank'>{link}</a>");
            }
            html.Append("</pre>");
            return html.ToString();
        }
    }
}