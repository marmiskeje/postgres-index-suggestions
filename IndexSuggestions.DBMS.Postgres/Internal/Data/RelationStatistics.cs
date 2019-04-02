using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class RelationStatistics : IRelationStatistics
    {
        [Column("relation_id")]
        public uint ID { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("seq_scan")]
        public long SeqScanCount { get; set; }
        [Column("seq_tup_read")]
        public long SeqTupleReadCount { get; set; }
        [Column("idx_scan")]
        public long IndexScanCount { get; set; }
        [Column("idx_tup_fetch")]
        public long IndexTupleFetchCount { get; set; }
        [Column("n_tup_ins")]
        public long TupleInsertCount { get; set; }
        [Column("n_tup_upd")]
        public long TupleUpdateCount { get; set; }
        [Column("n_tup_del")]
        public long TupleDeleteCount { get; set; }

        public bool Equals(IRelationStatistics other)
        {
            return ID == other.ID && DatabaseID == other.DatabaseID && SeqScanCount == other.SeqScanCount && SeqTupleReadCount == other.SeqTupleReadCount
                && IndexScanCount == other.IndexScanCount && IndexTupleFetchCount == other.IndexTupleFetchCount && IndexTupleFetchCount == other.IndexTupleFetchCount
                && TupleInsertCount == other.TupleInsertCount && TupleUpdateCount == other.TupleUpdateCount && TupleDeleteCount == other.TupleDeleteCount;
        }
    }
}
