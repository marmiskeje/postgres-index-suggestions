using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class AppSettings
    {
        public AnalysisSettings AnalysisSettings { get; set; }
    }
    internal class AnalysisSettings
    {
        public int HPartitioningMinRowsCount { get; set; }
        public decimal HPartitioningMinTotalImprovementRatio { get; set; }
        public int IndexMaxAttributesCount { get; set; }
    }
}
