using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class SummaryTotalRelationStatistics
    {
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public long SeqScanCount { get; set; }
        public long SeqTupleReadCount { get; set; }
        public long IndexScanCount { get; set; }
        public long IndexTupleFetchCount { get; set; }
        public long TupleInsertCount { get; set; }
        public long TupleUpdateCount { get; set; }
        public long TupleDeleteCount { get; set; }
        public long TotalLivenessCount { get; set; }
    }
}
