using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedStatementStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        public uint DatabaseID { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public long TotalExecutionsCount { get; set; }
#warning Tu este navrhujem, aby si mohol pouzivatel nastavit zber nielen per db, ale aj per command (Select, insert, update, ...) - aby sa dali ignorovat inserty, etc.
        public string RepresentativeStatement { get; set; }
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxDuration { get; set; }
        public TimeSpan AvgDuration { get; set; }
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
