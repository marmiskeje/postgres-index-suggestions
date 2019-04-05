using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class StoredProcedure : IStoredProcedure
    {
        [Column("proc_id")]
        public uint ID { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("proc_name")]
        public string Name { get; set; }
        [Column("schema_id")]
        public uint SchemaID { get; set; }
        [Column("schema_name")]
        public string SchemaName { get; set; }
        [Column("lang_name")]
        public string LanguageName { get; set; }
        [Column("proc_args_count")]
        public int ArgumentsCount { get; set; }
        [Column("proc_definition")]
        public string Definition { get; set; }
    }
}
