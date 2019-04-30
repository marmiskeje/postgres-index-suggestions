using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class TotalRelationStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public uint RelationID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long SeqScanCount { get; set; }
        [Required]
        public long SeqTupleReadCount { get; set; }
        [Required]
        public long IndexScanCount { get; set; }
        [Required]
        public long IndexTupleFetchCount { get; set; }
        [Required]
        public long TupleInsertCount { get; set; }
        [Required]
        public long TupleUpdateCount { get; set; }
        [Required]
        public long TupleDeleteCount { get; set; }
    }

    public class TotalRelationStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime Date { get; set; }
    }
}
