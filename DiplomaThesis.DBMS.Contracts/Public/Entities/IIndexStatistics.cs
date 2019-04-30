using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IIndexStatistics : IEquatable<IIndexStatistics>
    {
        uint ID { get; set; }
        uint RelationID { get; set; }
        uint DatabaseID { get; set; }
        long IndexScanCount { get; set; }
        long IndexTupleReadCount { get; set; }
        long IndexTupleFetchCount { get; set; }
    }
}
