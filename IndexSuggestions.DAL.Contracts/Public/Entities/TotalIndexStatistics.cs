using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class TotalIndexStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public uint RelationID { get; set; }
        [Required]
        public uint IndexID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long IndexScanCount { get; set; }
        [Required]
        public long IndexTupleReadCount { get; set; }
        [Required]
        public long IndexTupleFetchCount { get; set; }
    }

    public class TotalIndexStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public uint IndexID { get; set; }
        public DateTime Date { get; set; }
    }
}
