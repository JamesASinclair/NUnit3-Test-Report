using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}