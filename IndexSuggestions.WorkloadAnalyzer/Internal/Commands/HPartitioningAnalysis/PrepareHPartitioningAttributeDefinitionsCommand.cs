using IndexSuggestions.Common;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class PrepareHPartitioningAttributeDefinitionsCommand : ChainableCommand
    {
        private const int MIN_RELATION_TUPLES_COUNT_FOR_PARTITIONING = 1000000;
        private readonly WorkloadAnalysisContext context;
        private readonly IAttributeHPartitioningDesigner attributeHPartitioningDesigner;
        public PrepareHPartitioningAttributeDefinitionsCommand(WorkloadAnalysisContext context, IAttributeHPartitioningDesigner attributeHPartitioningDesigner)
        {
            this.context = context;
            this.attributeHPartitioningDesigner = attributeHPartitioningDesigner;
        }
        protected override void OnExecute()
        {
            Dictionary<AttributeData, HashSet<string>> attributesAndTheirOperators = new Dictionary<AttributeData, HashSet<string>>();
            foreach (var kv in context.StatementsData.MostSignificantSelectQueriesByRelation)
            {
                var relationID = kv.Key;
                if (context.RelationsData.TryGetRelation(relationID, out var relation))
                {
                    if (relation.TuplesCount >= MIN_RELATION_TUPLES_COUNT_FOR_PARTITIONING)
                    {
                        var statementQueries = kv.Value;
                        foreach (var statementQuery in statementQueries)
                        {
                            var extractedData = context.StatementsExtractedData.DataPerQuery[statementQuery];
                            var clausulesAndTheirData = new List<IReadOnlyDictionary<AttributeData, ISet<string>>>();
                            clausulesAndTheirData.Add(extractedData.WhereOperatorsByAttribute);
                            clausulesAndTheirData.Add(extractedData.JoinOperatorsByAttribute);
                            foreach (var operatorsByAttribute in clausulesAndTheirData)
                            {
                                foreach (var kv2 in operatorsByAttribute)
                                {
                                    var attribute = kv2.Key;
                                    var operators = kv2.Value;
                                    if (!attributesAndTheirOperators.ContainsKey(attribute))
                                    {
                                        attributesAndTheirOperators.Add(attribute, new HashSet<string>());
                                    }
                                    attributesAndTheirOperators[attribute].AddRange(operators);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var kv in attributesAndTheirOperators)
            {
                var attribute = kv.Key;
                var operators = kv.Value;
                var targetRelation = context.RelationsData.GetReplacementOrOriginal(attribute.Relation.ID);
                context.RelationsData.TryGetPrimaryKey(targetRelation.ID, out var primaryKey);
                var hPartitioning = attributeHPartitioningDesigner.DesignFor(attribute.WithReplacedRelation(targetRelation), operators, primaryKey);
                if (hPartitioning != null)
                {
                    context.HPartitioningDesignData.AttributesPartitioning.Add(attribute, hPartitioning);
                }
            }
        }
    }
}
