using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal static class NormalizedStatementRelationStatisticsMergeUtility
    {
        public static void ApplySample(NormalizedStatementRelationStatistics cumulativeData, NormalizedStatementRelationStatistics newSample)
        {
            cumulativeData.MaxTotalCost = Math.Max(cumulativeData.MaxTotalCost, newSample.MaxTotalCost);
            cumulativeData.MinTotalCost = Math.Min(cumulativeData.MinTotalCost, newSample.MinTotalCost);
            if (cumulativeData.TotalScansCount > 0 || newSample.TotalScansCount > 0)
            {
                cumulativeData.AvgTotalCost = (cumulativeData.TotalScansCount * cumulativeData.AvgTotalCost
                                              + newSample.TotalScansCount * newSample.AvgTotalCost
                                              )
                                              / (cumulativeData.TotalScansCount + newSample.TotalScansCount);
            }
            else
            {
                cumulativeData.AvgTotalCost = newSample.AvgTotalCost;
            }
            cumulativeData.TotalScansCount += newSample.TotalScansCount;
        }
    }
}
