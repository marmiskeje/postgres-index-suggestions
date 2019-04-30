using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class TotalStoredProcedureStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public uint ProcedureID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CallsCount { get; set; }
        [Required]
        public decimal TotalDurationInMs { get; set; }
        [Required]
        public decimal SelfDurationInMs { get; set; }
    }

    public class TotalStoredProcedureStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint ProcedureID { get; set; }
        public DateTime Date { get; set; }
    }
}
