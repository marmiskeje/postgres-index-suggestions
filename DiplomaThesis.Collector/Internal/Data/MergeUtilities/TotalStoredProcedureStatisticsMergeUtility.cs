using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class TotalStoredProcedureStatisticsMergeUtility
    {
        public static void ApplySample(TotalStoredProcedureStatistics cumulativeData, TotalStoredProcedureStatistics newSample)
        {
            cumulativeData.CallsCount += newSample.CallsCount;
            cumulativeData.SelfDurationInMs = cumulativeData.SelfDurationInMs + newSample.SelfDurationInMs;
            cumulativeData.TotalDurationInMs = cumulativeData.TotalDurationInMs + newSample.TotalDurationInMs;
        }
    }
}
