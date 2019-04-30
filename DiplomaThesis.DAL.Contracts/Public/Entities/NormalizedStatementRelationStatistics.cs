using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class NormalizedStatementRelationStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public uint RelationID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long TotalScansCount { get; set; }
        [Required]
        public decimal MinTotalCost { get; set; }
        [Required]
        public decimal MaxTotalCost { get; set; }
        [Required]
        public decimal AvgTotalCost { get; set; }
    }

    public class NormalizedStatementRelationStatisticsUniqueKey
    {
        public long NormalizedStatementID { get; set; }
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime Date { get; set; }
    }
}
