using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class TotalIndexStatisticsMergeUtility
    {
        public static void ApplySample(TotalIndexStatistics cumulativeData, TotalIndexStatistics newSample)
        {
            cumulativeData.IndexScanCount += newSample.IndexScanCount;
            cumulativeData.IndexTupleFetchCount += newSample.IndexTupleFetchCount;
            cumulativeData.IndexTupleReadCount += newSample.IndexTupleReadCount;
        }
    }
}
