using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class IndexAttribute : IEquatable<IndexAttribute>
    {
        private readonly string identificationString;
        public IndexRelation Relation { get; }
        public string Name { get; }
        public DbType DbType { get; }
        public decimal CardinalityIndicator { get; }
        public object[] MostCommonValues { get; }
        public decimal[] MostCommonValuesFrequencies { get; }

        public IndexAttribute(IndexRelation relation, string name, DbType dbType, decimal cardinalityIndicator, object[] mostCommonVals, decimal[] mostCommonValsFreqs)
        {
            Relation = relation;
            Name = name;
            DbType = dbType;
            CardinalityIndicator = cardinalityIndicator;
            MostCommonValues = mostCommonVals;
            MostCommonValuesFrequencies = mostCommonValsFreqs;
            this.identificationString = $"{Relation.ID.ToString() ?? "UnknownRelation"}_{Name}";
        }

        public override int GetHashCode()
        {
            return identificationString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IndexAttribute))
            {
                return false;
            }
            return Equals((IndexAttribute)obj);
        }

        public bool Equals(IndexAttribute tmp)
        {
            if (tmp == null)
            {
                return false;
            }
            return string.Equals(identificationString, tmp.identificationString);
        }
    }
}
