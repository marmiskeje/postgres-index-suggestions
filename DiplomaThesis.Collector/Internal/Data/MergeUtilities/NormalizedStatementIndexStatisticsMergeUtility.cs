using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class NormalizedStatementIndexStatisticsMergeUtility
    {
        public static void ApplySample(NormalizedStatementIndexStatistics cumulativeData, NormalizedStatementIndexStatistics newSample)
        {
            cumulativeData.MaxTotalCost = Math.Max(cumulativeData.MaxTotalCost, newSample.MaxTotalCost);
            cumulativeData.MinTotalCost = Math.Min(cumulativeData.MinTotalCost, newSample.MinTotalCost);
            if (cumulativeData.TotalIndexScanCount > 0 || newSample.TotalIndexScanCount > 0)
            {
                cumulativeData.AvgTotalCost = (cumulativeData.TotalIndexScanCount * cumulativeData.AvgTotalCost
                                              + newSample.TotalIndexScanCount * newSample.AvgTotalCost
                                              )
                                              / (cumulativeData.TotalIndexScanCount + newSample.TotalIndexScanCount);
            }
            else
            {
                cumulativeData.AvgTotalCost = newSample.AvgTotalCost;
            }
            cumulativeData.TotalIndexScanCount += newSample.TotalIndexScanCount;
        }
    }
}
