using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelationStatistics : IEquatable<IRelationStatistics>
    {
        uint ID { get; set; }
        uint DatabaseID { get; set; }
        long SeqScanCount { get; set; }
        long SeqTupleReadCount { get; set; }
        long IndexScanCount { get; set; }
        long IndexTupleFetchCount { get; set; }
        long TupleInsertCount { get; set; }
        long TupleUpdateCount { get; set; }
        long TupleDeleteCount { get; set; }
    }
}
