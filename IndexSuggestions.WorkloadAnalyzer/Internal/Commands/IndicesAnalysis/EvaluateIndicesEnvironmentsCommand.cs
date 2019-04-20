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
        private readonly IIndexSqlCreateStatementGenerator indexSqlCreateStatementGenerator;

        public EvaluateIndicesEnvironmentsCommand(WorkloadAnalysisContext context, IVirtualIndicesRepository virtualIndicesRepository, IExplainRepository explainRepository,
                                             IIndexSqlCreateStatementGenerator indexSqlCreateStatementGenerator)
        {
            this.context = context;
            this.virtualIndicesRepository = virtualIndicesRepository;
            this.explainRepository = explainRepository;
            this.indexSqlCreateStatementGenerator = indexSqlCreateStatementGenerator;
        }

        protected override void OnExecute()
        {
            context.IndicesDesignData.PossibleIndexSizes.Clear();
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var env in context.IndicesDesignData.IndicesEnvironments)
                {
                    try
                    {
                        virtualIndicesRepository.DestroyAll();
                        var virtualIndicesMapping = new Dictionary<IndexDefinition, IVirtualIndex>();
                        foreach (var index in env.PossibleIndices.All)
                        {
                            var virtualIndex = virtualIndicesRepository.Create(new VirtualIndexDefinition()
                            {
                                CreateStatement = indexSqlCreateStatementGenerator.Generate(index)
                            });
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
                            var normalizedStatement = context.Statements[statementID].NormalizedStatement;
                            var representativeStatement = context.Statements[statementID].RepresentativeStatistics.RepresentativeStatement;
                            var explainResult = explainRepository.Eplain(representativeStatement);
                            var latestPlan = explainResult.Plan;
                            HashSet<IndexDefinition> improvingVirtualIndices = new HashSet<IndexDefinition>();
                            
                            env.PlansPerStatement.Add(statementID, explainResult);
                            if (latestPlan.TotalCost < context.IndicesDesignData.RealExecutionPlansForStatements[statementID].Plan.TotalCost)
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