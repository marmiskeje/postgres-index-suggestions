﻿using DiplomaThesis.Common;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class PrepareHPartitioningAttributeDefinitionsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IAttributeHPartitioningDesigner attributeHPartitioningDesigner;
        private readonly AnalysisSettings settings;
        public PrepareHPartitioningAttributeDefinitionsCommand(WorkloadAnalysisContext context, IAttributeHPartitioningDesigner attributeHPartitioningDesigner, AnalysisSettings settings)
        {
            this.context = context;
            this.attributeHPartitioningDesigner = attributeHPartitioningDesigner;
            this.settings = settings;
        }
        protected override void OnExecute()
        {
            Dictionary<AttributeData, HashSet<string>> attributesAndTheirOperators = new Dictionary<AttributeData, HashSet<string>>();
            foreach (var kv in context.StatementsData.MostSignificantSelectQueriesByRelation)
            {
                var relationID = kv.Key;
                if (context.RelationsData.TryGetRelation(relationID, out var relation) && !context.Workload.Definition.Relations.ForbiddenValues.Contains(relationID))
                {
                    if (relation.TuplesCount >= settings.HPartitioningMinRowsCount)
                    {
                        var statementQueries = kv.Value;
                        foreach (var statementQuery in statementQueries)
                        {
                            var extractedData = context.StatementsExtractedData.DataPerQuery[statementQuery];
                            var clausulesAndTheirData = new List<IReadOnlyDictionary<AttributeData, ISet<string>>>();
                            clausulesAndTheirData.Add(extractedData.WhereAttributes.AllOperatorsByAttribute);
                            clausulesAndTheirData.Add(extractedData.JoinAttributes.AllOperatorsByAttribute);
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
