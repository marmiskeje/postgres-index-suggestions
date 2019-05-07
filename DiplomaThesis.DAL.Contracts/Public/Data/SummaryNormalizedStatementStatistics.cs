using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class SummaryNormalizedStatementStatistics
    {
        public string Statement { get; set; }
        public long TotalExecutionsCount { get; set; }
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxDuration { get; set; }
        public TimeSpan AvgDuration { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
}
