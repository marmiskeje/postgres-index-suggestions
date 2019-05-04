using DiplomaThesis.Common;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class NormalizedStatementStatisticsMergeUtility
    {
        public static void ApplySample(NormalizedStatementStatistics cumulativeData, NormalizedStatementStatistics newSample)
        {
            if (newSample.MaxDuration > cumulativeData.MaxDuration)
            {
                cumulativeData.RepresentativeStatement = newSample.RepresentativeStatement;
            }
            cumulativeData.MaxDuration = TimeSpanExtensions.Max(cumulativeData.MaxDuration, newSample.MaxDuration);
            cumulativeData.MinDuration = TimeSpanExtensions.Min(cumulativeData.MinDuration, newSample.MinDuration);
            if (cumulativeData.TotalExecutionsCount > 0 || newSample.TotalExecutionsCount > 0)
            {
                cumulativeData.AvgDuration = TimeSpan.FromMilliseconds(
                                                                        (
                                                                            cumulativeData.TotalExecutionsCount * cumulativeData.AvgDuration.TotalMilliseconds
                                                                            + newSample.TotalExecutionsCount * newSample.AvgDuration.TotalMilliseconds
                                                                        )
                                                                        / (cumulativeData.TotalExecutionsCount + newSample.TotalExecutionsCount)
                                                                      );
            }
            else
            {
                cumulativeData.AvgDuration = newSample.AvgDuration;
            }
            cumulativeData.TotalExecutionsCount += newSample.TotalExecutionsCount;
            cumulativeData.TotalDuration += newSample.TotalDuration;
            if (cumulativeData.MaxTotalCost.HasValue)
            {
                if (newSample.MaxTotalCost.HasValue)
                {
                    cumulativeData.MaxTotalCost = Math.Max(cumulativeData.MaxTotalCost.Value, newSample.MaxTotalCost.Value); 
                }
            }
            else
            {
                cumulativeData.MaxTotalCost = newSample.MaxTotalCost;
            }
            if (cumulativeData.MinTotalCost.HasValue)
            {
                if (newSample.MinTotalCost.HasValue)
                {
                    cumulativeData.MinTotalCost = Math.Max(cumulativeData.MinTotalCost.Value, newSample.MinTotalCost.Value);
                }
            }
            else
            {
                cumulativeData.MinTotalCost = newSample.MinTotalCost;
            }
            if (cumulativeData.AvgTotalCost.HasValue)
            {
                if (newSample.AvgTotalCost.HasValue && (cumulativeData.TotalExecutionsCount > 0 || newSample.TotalExecutionsCount > 0))
                {
                    cumulativeData.AvgTotalCost = (cumulativeData.TotalExecutionsCount * cumulativeData.AvgTotalCost.Value
                                                   + newSample.TotalExecutionsCount * newSample.AvgTotalCost.Value
                                                   )
                                                   / (cumulativeData.TotalExecutionsCount + newSample.TotalExecutionsCount);
                }
            }
            else
            {
                cumulativeData.AvgTotalCost = newSample.AvgTotalCost;
            }
        }
    }
}
