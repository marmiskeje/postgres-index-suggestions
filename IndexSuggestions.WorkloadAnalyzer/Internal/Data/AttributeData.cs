using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class AttributeData : IEquatable<AttributeData>
    {
        private readonly IRelationAttribute attribute;
        private readonly string identificationString;
        public RelationData Relation { get; }
        public string Name => attribute.Name;
        public DbType DbType => attribute.DbType;
        public decimal CardinalityIndicator => attribute.CardinalityIndicator;
        public object[] MostCommonValues => attribute.MostCommonValues;
        public decimal[] MostCommonValuesFrequencies => attribute.MostCommonValuesFrequencies;
        public bool IsNullable => attribute.IsNullable;
        public object[] HistogramBounds => attribute.HistogramBounds;
        public AttributeData(RelationData relation, IRelationAttribute attribute)
        {
            Relation = relation;
            this.attribute = attribute;
            this.identificationString = $"{Relation.ID.ToString() ?? "UnknownRelation"}_{Name}";
        }

        public override int GetHashCode()
        {
            return identificationString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is AttributeData))
            {
                return false;
            }
            return Equals((AttributeData)obj);
        }

        public bool Equals(AttributeData tmp)
        {
            if (tmp == null)
            {
                return false;
            }
            return string.Equals(identificationString, tmp.identificationString);
        }

        public AttributeData WithReplacedRelation(RelationData relation)
        {
            return new AttributeData(relation, attribute);
        }
    }
}
