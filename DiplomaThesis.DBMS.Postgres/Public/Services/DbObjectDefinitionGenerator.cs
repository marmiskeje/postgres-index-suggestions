using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiplomaThesis.DBMS.Postgres
{
    public class DbObjectDefinitionGenerator : IDbObjectDefinitionGenerator
    {
        private readonly bool supportsInclude;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        public DbObjectDefinitionGenerator(bool supportsInclude, IToSqlValueStringConverter toSqlValueStringConverter)
        {
            this.supportsInclude = supportsInclude;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
        }
        public VirtualIndexDefinition Generate(IndexDefinition indexDefinition, string filterExpression = null)
        {
            var relation = indexDefinition.Relation;
            var attributes = indexDefinition.Attributes.Select(x => x.Name);
            var includeAttributes = indexDefinition.IncludeAttributes.Select(x => x.Name);
            if (!supportsInclude)
            {
                attributes = attributes.Concat(includeAttributes);
            }
            var builder = new StringBuilder();
            builder.Append("CREATE INDEX ON");
            builder.Append($"{relation.SchemaName}.{relation.Name} ");
            builder.Append($"({ String.Join(", ", attributes)})");
            if (supportsInclude)
            {
                builder.Append($" INCLUDE({ String.Join(", ", includeAttributes)})");
            }
            if (!String.IsNullOrEmpty(filterExpression))
            {
                builder.Append($" WHERE {filterExpression}");
            }
            return new VirtualIndexDefinition() { CreateStatement = builder.ToString() };
        }

        public VirtualHPartitioningDefinition Generate(HPartitioningDefinition hPartitioningDefinition)
        {
            var result = new VirtualHPartitioningDefinition();
            var relation = hPartitioningDefinition.Relation;
            if (hPartitioningDefinition.PartitioningAttributes.First() is RangeHPartitioningAttributeDefinition)
            {
                var partitioningAttributes = hPartitioningDefinition.PartitioningAttributes.Cast<RangeHPartitioningAttributeDefinition>();
                result.PartitioningStatement = String.Format("PARTITION BY RANGE ({0})", String.Join(",", partitioningAttributes.Select(x => x.Attribute.Name)));
                GenerateRange(partitioningAttributes, new List<RangeHPartitionAttributeDefinition>(), relation, ref result);
            }
            else if (hPartitioningDefinition.PartitioningAttributes.First() is HashHPartitioningAttributeDefinition)
            {
                var partitioningAttributes = hPartitioningDefinition.PartitioningAttributes.Cast<HashHPartitioningAttributeDefinition>();
                result.PartitioningStatement = String.Format("PARTITION BY HASH ({0})", String.Join(",", partitioningAttributes.Select(x => x.Attribute.Name)));
                int partitionCounts = partitioningAttributes.Sum(x => x.Modulus);
                for (int i = 0; i < partitionCounts; i++)
                {
                    result.PartitionStatements.Add(String.Format("PARTITION OF {0}.{1} FOR VALUES WITH (MODULUS {2}, REMAINDER {3})",
                                                                    relation.SchemaName, relation.Name, partitionCounts, i
                                                                )
                                                  );
                }
            }
            result.RelationName = hPartitioningDefinition.Relation.Name;
            return result;
        }

        private void GenerateRange(IEnumerable<RangeHPartitioningAttributeDefinition> attributePartitions, List<RangeHPartitionAttributeDefinition> partitionParts,
                                   RelationData relation, ref VirtualHPartitioningDefinition result)
        {
            if (attributePartitions.Count() > 0)
            {
                var first = attributePartitions.First();
                foreach (var p in first.Partitions)
                {
                    var pp = new List<RangeHPartitionAttributeDefinition>(partitionParts);
                    pp.Add(p);
                    GenerateRange(attributePartitions.Skip(1), pp, relation, ref result);
                }
            }
            else
            {
                result.PartitionStatements.Add(String.Format("PARTITION OF {0}.{1} FOR VALUES FROM ({2}) TO ({3})",
                                                                relation.SchemaName, relation.Name,
                                                                String.Join(",", partitionParts.Select(x => toSqlValueStringConverter.Convert(x.DbType, x.FromValueInclusive))),
                                                                String.Join(",", partitionParts.Select(x => toSqlValueStringConverter.Convert(x.DbType, x.FromValueInclusive)))
                                                            )
                                              );
            }
        }
    }
}
