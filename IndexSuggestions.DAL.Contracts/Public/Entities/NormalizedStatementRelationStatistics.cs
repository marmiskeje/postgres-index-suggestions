using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedStatementRelationStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public long TotalScansCount { get; set; }
        public decimal MinTotalCost { get; set; }
        public decimal MaxTotalCost { get; set; }
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
