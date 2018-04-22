using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class RelationAttribute : IRelationAttribute
    {
        public uint ID { get; set; }
        [Column("attnum")]
        public int AttributeNumber { get; set; }
        [Column("attrelid")]
        public uint RelationID { get; set; }
        [Column("attname")]
        public string Name { get; set; }
    }
}
