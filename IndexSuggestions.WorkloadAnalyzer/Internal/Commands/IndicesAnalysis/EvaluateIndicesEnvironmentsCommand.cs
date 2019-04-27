using System;
using System.Linq;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    /// <summary>
    /// For each environment: evaluate virtual indices usage/improvement
    /// </summary>
    internal class EvaluateIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualIndicesRepository virtualIndicesRepository;
        private readonly IExplainRepository explainRepository;
        private readonly IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;

        public EvaluateIndicesEnvironmentsCommand(WorkloadAnalysisContext context, IVirtualIndicesRepository virtualIndicesRepository, IExplainRepository explainRepository,
                                             IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.context = context;
            this.virtualIndicesRepository = virtualIndicesRepository;
            this.explainRepository = explainRepository;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
        }

        protected override void OnExecute()
        {
            context.IndicesDesignData.PossibleIndexSizes.Clear();
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var env in context.IndicesDesignData.Environments)
                {
                    try
                    {
                        virtualIndicesRepository.DestroyAll();
                        var virtualIndicesMapping = new Dictionary<IndexDefinition, IVirtualIndex>();
                        foreach (var index in env.PossibleIndices.All)
                        {
                            var targetRelationData = context.RelationsData.GetReplacementOrOriginal(index.Relation.ID);
                            var virtualIndex = virtualIndicesRepository.Create(dbObjectDefinitionGenerator.Generate(index.WithReplacedRelation(targetRelationData)));
                            if (virtualIndex != null)
                            {
                                virtualIndicesMapping.Add(index, virtualIndex);
                            }
                            if (!context.IndicesDesignData.PossibleIndexSizes.ContainsKey(index))
                            {
                                context.IndicesDesignData.PossibleIndexSizes.Add(index, virtualIndicesRepository.GetVirtualIndexSize(virtualIndex.ID));
                            }
                        }
                        foreach (var kv in env.PossibleIndices.AllPerStatement)
                        {
                            var statementID = kv.Key;
                            var indices = kv.Value;
                            var normalizedStatement = context.StatementsData.AllSelects[statementID].NormalizedStatement;
                            var representativeStatement = context.StatementsData.AllSelects[statementID].RepresentativeStatistics.RepresentativeStatement;
                            var statementToUse = RepresentativeStatementReplacementUtility.Provide(normalizedStatement, representativeStatement, context.RelationsData);
                            var explainResult = explainRepository.Eplain(statementToUse);
                            var latestPlan = explainResult.Plan;
                            HashSet<IndexDefinition> improvingVirtualIndices = new HashSet<IndexDefinition>();
                            
                            env.PlansPerStatement.Add(statementID, explainResult);
                            if (latestPlan.TotalCost < context.RealExecutionPlansForStatements[statementID].Plan.TotalCost)
                            {
                                foreach (var i in indices)
                                {
                                    var virtualIndex = virtualIndicesMapping[i];
                                    if (explainResult.UsedIndexScanIndices.Contains(virtualIndex.Name))
                                    {
                                        improvingVirtualIndices.Add(i);
                                    }
                                }
                            }
                            foreach (var query in normalizedStatement.StatementDefinition.IndependentQueries)
                            {
                                var queryKey = new NormalizedStatementQueryPair(statementID, query);
                                env.ImprovingPossibleIndices.TryAddPossibleIndices(improvingVirtualIndices, normalizedStatement, query);
                                env.ImprovingPossibleIndices.TryAddPossibleCoveringIndices(env.PossibleIndices.AllPerQuery[queryKey].Intersect(improvingVirtualIndices), normalizedStatement, query);
                            }
                        }
                    }
                    finally
                    {
                        virtualIndicesRepository.DestroyAll();
                    }
                }

            }
        }
    }
}