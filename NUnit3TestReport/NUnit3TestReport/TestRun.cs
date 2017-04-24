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
    }
}