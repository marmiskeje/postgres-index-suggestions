using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class TotalRelationStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public long SeqScanCount { get; set; }
        public long SeqTupleReadCount { get; set; }
        public long IndexScanCount { get; set; }
        public long IndexTupleFetchCount { get; set; }
        public long TupleInsertCount { get; set; }
        public long TupleUpdateCount { get; set; }
        public long TupleDeleteCount { get; set; }
    }

    public class TotalRelationStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime Date { get; set; }
    }
}
