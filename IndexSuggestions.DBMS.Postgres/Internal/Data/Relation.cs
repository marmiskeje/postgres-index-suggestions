using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class Relation : IRelation
    {
        [Column("relation_id")]
        public uint ID { get; set; }
        [Column("relation_name")]
        public string Name { get; set; }
        [Column("schema_id")]
        public uint SchemaID { get; set; }
        [Column("schema_name")]
        public string SchemaName { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("db_name")]
        public string DatabaseName { get; set; }
    }
}
