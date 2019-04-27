using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal static class RepresentativeStatementReplacementUtility
    {
        public static string Provide(NormalizedStatement normalizedStatement, string representativeStatement, WorkloadRelationsData relationsData)
        {
            string result = representativeStatement;
            Dictionary<string, string> replacementMapping = new Dictionary<string, string>();
            foreach (var query in normalizedStatement.StatementDefinition.IndependentQueries)
            {
                foreach (var relation in query.Relations)
                {
                    if (relationsData.TryGetRelation(relation.ID, out var sourceRelation))
                    {
                        var targetRelation = relationsData.GetReplacementOrOriginal(relation.ID);
                        if (targetRelation.ID != sourceRelation.ID)
                        {
                            replacementMapping.Add($"{sourceRelation.SchemaName}.{sourceRelation.Name}", $"{targetRelation.SchemaName}.{targetRelation.Name}");
                        } 
                    }
                }
            }
            foreach (var kv in replacementMapping)
            {
                var source = kv.Key;
                var target = kv.Value;
                result = result.Replace(source, target);
            }
            return result;
        }
    }
}
