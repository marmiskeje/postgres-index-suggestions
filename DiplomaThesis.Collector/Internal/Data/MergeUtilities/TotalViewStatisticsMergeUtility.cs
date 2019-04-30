using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class TotalViewStatisticsMergeUtility
    {
        public static void ApplySample(TotalViewStatistics cumulativeData, TotalViewStatistics newSample)
        {
            cumulativeData.RewrittenCount += newSample.RewrittenCount;
        }
    }
}
