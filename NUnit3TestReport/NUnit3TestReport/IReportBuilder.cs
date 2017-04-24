using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit3TestReport
{
    public interface IReportBuilder
    {
        string Build(List<TestRun> testRuns);
    }
}