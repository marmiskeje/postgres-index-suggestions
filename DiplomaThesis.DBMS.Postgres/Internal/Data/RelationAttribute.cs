using DiplomaThesis.Common;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class RelationAttribute : IRelationAttribute
    {
        private string operators = null;
        private IEnumerable<string> supportedOperators = new HashSet<string>();
        private float[] mostCommonValuesFrequenciesFloat = null;
        private decimal[] mostCommonValuesFrequencies = null;
        private string histogramBoundsString = null;
        private string[] histogramBounds = null;
        private string mostCommonValuesString = null;
        private string[] mostCommonValues = null;
        [Column("attr_number")]
        public int AttributeNumber { get; set; }
        [Column("attr_rel_id")]
        public uint RelationID { get; set; }
        [Column("attr_name")]
        public string Name { get; set; }
        [Column("attr_operators")]
        public string Operators
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
        public string MostCommonValuesString
        {
            get
            {
                return mostCommonValuesString;
            }
            set
            {
                mostCommonValuesString = value;
                if (value != null && value.Length > 0)
                {
                    mostCommonValues = value.Split("¤", StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        public string[] MostCommonValues
        {
            get { return mostCommonValues; }
        }
        [Column("attr_stats_most_common_freqs")]
        public float[] MostCommonValuesFrequenciesFloat
        {
            get
            {
                return mostCommonValuesFrequenciesFloat;
            }
            set
            {
                mostCommonValuesFrequenciesFloat = value;
                if (value != null)
                {
                    mostCommonValuesFrequencies = value.Select(x => Convert.ToDecimal(x)).ToArray();
                }
            }
        }
        public decimal[] MostCommonValuesFrequencies
        {
            get { return mostCommonValuesFrequencies; }
        }
        [Column("attr_typeid")]
        public uint DbTypeId { get; set; }
        public DbType DbType
        {
            get { return PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(DbTypeId, x => x.OID)); }
        }
        [Column("attr_is_nullable")]
        public bool IsNullable { get; set; }
        [Column("attr_stats_histogram_bounds")]
        public string HistogramBoundsString
        {
            get
            {
                return histogramBoundsString;
            }
            set
            {
                histogramBoundsString = value;
                if (value != null && value.Length > 0)
                {
                    histogramBounds = value.Split("¤", StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        public string[] HistogramBounds
        {
            get { return histogramBounds; }
        }
    }
}
