using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class View : IView
    {
        [Column("view_id")]
        public uint ID { get; set; }
        [Column("view_name")]
        public string Name { get; set; }
        [Column("schema_name")]
        public string SchemaName { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("view_definition")]
        public string Definition { get; set; }
    }
}
