using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class NormalizedStatementStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long TotalExecutionsCount { get; set; }
#warning Tu este navrhujem, aby si mohol pouzivatel nastavit zber nielen per db, ale aj per command (Select, insert, update, ...) - aby sa dali ignorovat inserty, etc.
        [Required]
        public string RepresentativeStatement { get; set; }
        [Required]
        public TimeSpan MinDuration { get; set; }
        [Required]
        public TimeSpan MaxDuration { get; set; }
        [Required]
        public TimeSpan AvgDuration { get; set; }
        [Required]
        public TimeSpan TotalDuration { get; set; }
        public decimal? MinTotalCost { get; set; }
        public decimal? MaxTotalCost { get; set; }
        public decimal? AvgTotalCost { get; set; }
    }

    public class NormalizedStatementStatisticsUniqueKey
    {
        public long NormalizedStatementID { get; set; }
        public uint DatabaseID { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Date { get; set; }
    }
}
