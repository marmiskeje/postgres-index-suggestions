using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class IndexStatistics : IIndexStatistics
    {
        [Column("index_id")]
        public uint ID { get; set; }
        [Column("relation_id")]
        public uint RelationID { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("idx_scan")]
        public long IndexScanCount { get; set; }
        [Column("idx_tup_read")]
        public long IndexTupleReadCount { get; set; }
        [Column("idx_tup_fetch")]
        public long IndexTupleFetchCount { get; set; }

        public bool Equals(IIndexStatistics other)
        {
            return ID == other.ID && RelationID == other.RelationID && DatabaseID == other.DatabaseID
                && IndexScanCount == other.IndexScanCount && IndexTupleFetchCount == other.IndexTupleFetchCount && IndexTupleReadCount == other.IndexTupleReadCount;
        }
    }
}
