using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class Database : IDatabase
    {
        [Column("db_id")]
        public uint ID { get; set; }
        [Column("db_name")]
        public string Name { get; set; }
    }
}
