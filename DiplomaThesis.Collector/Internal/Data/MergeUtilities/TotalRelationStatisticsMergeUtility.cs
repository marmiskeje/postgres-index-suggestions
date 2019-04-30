using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal static class TotalRelationStatisticsMergeUtility
    {
        public static void ApplySample(TotalRelationStatistics cumulativeData, TotalRelationStatistics newSample)
        {
            cumulativeData.IndexScanCount += newSample.IndexScanCount;
            cumulativeData.IndexTupleFetchCount += newSample.IndexTupleFetchCount;
            cumulativeData.SeqScanCount += newSample.SeqScanCount;
            cumulativeData.SeqTupleReadCount += newSample.SeqTupleReadCount;
            cumulativeData.TupleDeleteCount += newSample.TupleDeleteCount;
            cumulativeData.TupleInsertCount += newSample.TupleInsertCount;
            cumulativeData.TupleUpdateCount += newSample.TupleUpdateCount;
        }
    }
}
