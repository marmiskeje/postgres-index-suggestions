using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class Index : IIndex
    {
        private string attributesNamesArray;
        [Column("index_id")]
        public uint ID { get; set; }
        [Column("index_name")]
        public string Name { get; set; }
        [Column("index_attributes")]
        public String AttributesNamesArray
        {
            get { return attributesNamesArray; }
            set
            {
                attributesNamesArray = value;
                if (value != null)
                {
                    var attributes = value.Split(",");
                    AttributesNames = new List<string>(attributes);
                }
            }
        }
        public IList<string> AttributesNames { get; set; }
        [Column("relation_id")]
        public uint RelationID { get; set; }
        [Column("relation_name")]
        public string RelationName { get; set; }
        [Column("schema_id")]
        public uint SchemaID { get; set; }
        [Column("schema_name")]
        public string SchemaName { get; set; }
        [Column("db_id")]
        public uint DatabaseID { get; set; }
        [Column("db_name")]
        public string DatabaseName { get; set; }

        public Index()
        {
            AttributesNames = new List<string>();
        }
    }
}
