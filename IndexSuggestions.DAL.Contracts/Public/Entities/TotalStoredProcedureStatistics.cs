using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class TotalStoredProcedureStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public uint DatabaseID { get; set; }
        public uint ProcedureID { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CallsCount { get; set; }
        public decimal TotalDurationInMs { get; set; }
        public decimal SelfDurationInMs { get; set; }
    }

    public class TotalStoredProcedureStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint ProcedureID { get; set; }
        public DateTime Date { get; set; }
    }
}
