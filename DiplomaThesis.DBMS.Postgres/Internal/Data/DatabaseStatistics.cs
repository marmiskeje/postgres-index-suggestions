using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class DatabaseStatistics : IDatabaseStatistics
    {
        [Column("db_id")]
        public uint ID { get; set; }
        [Column("db_stats_reset")]
        public DateTime? LastResetDate { get; set; }
    }
}
