using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class StoredProcedureStatistics : IStoredProcedureStatistics
    {
        [Column("proc_id")]
        public uint ID { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("calls")]
        public long CallsCount { get; set; }
        [Column("total_time")]
        public decimal TotalDurationInMs { get; set; }
        [Column("self_time")]
        public decimal SelfDurationInMs { get; set; }

        public bool Equals(IStoredProcedureStatistics other)
        {
            return ID == other.ID && DatabaseID == other.DatabaseID && CallsCount == other.CallsCount
                && TotalDurationInMs == other.TotalDurationInMs && SelfDurationInMs == other.SelfDurationInMs;
        }
    }
}
