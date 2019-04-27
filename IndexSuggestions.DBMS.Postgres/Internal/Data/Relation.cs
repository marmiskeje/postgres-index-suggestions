using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class Relation : IRelation
    {
        private string primaryKeyAttributeNamesArray;
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
        [Column("relation_size")]
        public long Size { get; set; }
        [Column("relation_tuples_count")]
        public long TuplesCount { get; set; }
        [Column("primary_key_attributes")]
        public String PrimaryKeyAttributeNamesArray
        {
            get { return primaryKeyAttributeNamesArray; }
            set
            {
                primaryKeyAttributeNamesArray = value;
                if (value != null)
                {
                    var attributes = value.Split(",");
                    PrimaryKeyAttributeNames = new List<string>(attributes);
                }
            }
        }
        public IList<string> PrimaryKeyAttributeNames { get; set; }
    }
}
