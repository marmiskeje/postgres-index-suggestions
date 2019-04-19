using IndexSuggestions.Common;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class RelationAttribute : IRelationAttribute
    {
        private string operators = null;
        private IEnumerable<string> supportedOperators = new HashSet<string>();
        [Column("attr_number")]
        public int AttributeNumber { get; set; }
        [Column("attr_relation_id")]
        public uint RelationID { get; set; }
        [Column("attr_name")]
        public string Name { get; set; }
        [Column("attr_operators")]
        internal string Operators
        {
            get { return operators; }
            set
            {
                operators = value;
                supportedOperators = new HashSet<string>((value ?? String.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public IEnumerable<string> SupportedOperators
        {
            get { return supportedOperators; }
            set
            {
                supportedOperators = value;
                operators = String.Join(",", value ?? new HashSet<string>());
            }
        }
        [Column("attr_stats_n_distinct")]
        public decimal CardinalityIndicator { get; set; }

        [Column("attr_stats_most_common_vals")]
        public object[] MostCommonValues { get; set; }
        [Column("attr_stats_most_common_freqs")]
        public decimal[] MostCommonValuesFrequencies { get; set; }
        [Column("attr_typeid")]
        public uint DbTypeId { get; set; }
        public DbType DbType
        {
            get { return PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(DbTypeId, x => x.OID)); }
        }
    }
}
