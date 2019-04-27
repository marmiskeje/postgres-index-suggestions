﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class HPartitioningDefinition
    {
        public RelationData Relation { get; }
        public List<HPartitioningAttributeDefinition> PartitioningAttributes { get; } = new List<HPartitioningAttributeDefinition>();

        public HPartitioningDefinition(RelationData relation)
        {
            Relation = relation;
        }

        public HPartitioningDefinition WithReplacedRelation(RelationData relation)
        {
            var result = new HPartitioningDefinition(relation);
            result.PartitioningAttributes.AddRange(PartitioningAttributes);
            return result;
        }
    }
    internal abstract class HPartitioningAttributeDefinition
    {
        public AttributeData Attribute { get; }
        public HPartitioningAttributeDefinition(AttributeData attribute)
        {
            Attribute = attribute;
        }
    }

    internal class RangeHPartitioningAttributeDefinition : HPartitioningAttributeDefinition
    {
        public List<RangeHPartitionAttributeDefinition> Partitions { get; } = new List<RangeHPartitionAttributeDefinition>();
        public RangeHPartitioningAttributeDefinition(AttributeData attribute) : base(attribute)
        {
            
        }
    }

    internal class HashHPartitioningAttributeDefinition : HPartitioningAttributeDefinition
    {
        public int Modulus { get; }
        public HashHPartitioningAttributeDefinition(AttributeData attribute, int modulus) : base(attribute)
        {
            Modulus = modulus;
        }
    }

    internal class RangeHPartitionAttributeDefinition
    {
        public DbType DbType { get; }
        public object FromValueInclusive { get; } 
        public object ToValueExclusive { get; }

        public RangeHPartitionAttributeDefinition(DbType dbType, object fromValueInclusive, object toValueExclusive)
        {
            DbType = dbType;
            FromValueInclusive = fromValueInclusive;
            ToValueExclusive = toValueExclusive;
        }
    }
}
