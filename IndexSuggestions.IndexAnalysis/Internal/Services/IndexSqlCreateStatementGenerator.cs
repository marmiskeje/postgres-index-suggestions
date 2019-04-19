using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class IndexSqlCreateStatementGenerator : IIndexSqlCreateStatementGenerator
    {
        private readonly bool supportsInclude;
        public IndexSqlCreateStatementGenerator(bool supportsInclude)
        {
            this.supportsInclude = supportsInclude;
        }
        public string Generate(IndexDefinition indexDefinition)
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
            return builder.ToString();
        }
    }
}
