using System;
using System.Linq;
using System.Collections.Generic;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using DiplomaThesis.Common.Logging;

namespace DiplomaThesis.WorkloadAnalyzer
{
    /// <summary>
    /// For each environment: evaluate virtual indices usage/improvement
    /// </summary>
    internal class EvaluateIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualIndicesRepository virtualIndicesRepository;
        private readonly IExplainRepository explainRepository;
        private readonly IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;

        public EvaluateIndicesEnvironmentsCommand(ILog log, WorkloadAnalysisContext context, IVirtualIndicesRepository virtualIndicesRepository, IExplainRepository explainRepository,
                                             IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.log = log;
            this.context = context;
            this.virtualIndicesRepository = virtualIndicesRepository;
            this.explainRepository = explainRepository;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
        }

        protected override void OnExecute()
        {
            context.IndicesDesignData.PossibleIndexSizes.Clear();
#if DEBUG
            int counter = 0;
#endif
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var env in context.IndicesDesignData.Environments)
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        ++counter;
                        if (counter % 10 == 0)
                        {
                            Console.WriteLine(((counter / (double)context.IndicesDesignData.Environments.Count) * 100).ToString("F2") + "%");
                        }
                    }
#endif
                    try
                    {
                        virtualIndicesRepository.DestroyAll();
                        var virtualIndicesMapping = new Dictionary<IndexDefinition, IVirtualIndex>();
                        bool indicesCreated = false;
                        try
                        {
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
                            indicesCreated = true;
                        }
                        catch (Exception ex)
                        {
                            log.Write(ex);
                        }
                        if (indicesCreated)
                        {
                            foreach (var kv in env.PossibleIndices.AllPerStatement)
                            {
                                var statementID = kv.Key;
                                var indices = kv.Value;
                                var workloadStatement = context.StatementsData.All[statementID];
                                var normalizedStatement = workloadStatement.NormalizedStatement;
                                var representativeStatement = workloadStatement.RepresentativeStatistics.RepresentativeStatement;
                                var statementToUse = RepresentativeStatementReplacementUtility.Provide(normalizedStatement, representativeStatement, context.RelationsData);
                                var explainResult = explainRepository.Eplain(statementToUse);
                                var latestPlan = explainResult.Plan;
                                var realPlan = context.RealExecutionPlansForStatements[statementID].Plan;
                                HashSet<IndexDefinition> improvingVirtualIndices = new HashSet<IndexDefinition>();
                                VirtualEnvironmentStatementEvaluation statementEvaluation = new VirtualEnvironmentStatementEvaluation();
                                statementEvaluation.ExecutionPlan = explainResult;
                                statementEvaluation.LocalImprovementRatio = 1m - (realPlan.TotalCost > 0 ? latestPlan.TotalCost / realPlan.TotalCost : 0m);
                                decimal statementPortion = context.StatementsData.AllExecutionsCount > 0 ? workloadStatement.TotalExecutionsCount / (decimal)context.StatementsData.AllExecutionsCount : 0m;
                                statementEvaluation.GlobalImprovementRatio = statementEvaluation.LocalImprovementRatio * statementPortion;
                                env.StatementsEvaluation.Add(statementID, statementEvaluation);

                                
                                foreach (var i in indices)
                                {
                                    var virtualIndex = virtualIndicesMapping[i];
                                    if (explainResult.UsedIndexScanIndices.Contains(virtualIndex.Name))
                                    {
                                        if (latestPlan.TotalCost < realPlan.TotalCost)
                                        {
                                            improvingVirtualIndices.Add(i);
                                            if (!env.IndicesEvaluation.ContainsKey(i))
                                            {
                                                env.IndicesEvaluation.Add(i, new VirtualIndicesEnvironmentIndexEvaluation());
                                            }
                                        }
                                        env.IndicesEvaluation[i].ImprovementRatio += statementEvaluation.GlobalImprovementRatio;
                                    }
                                }
                                foreach (var query in normalizedStatement.StatementDefinition.IndependentQueries)
                                {
                                    var queryKey = new NormalizedStatementQueryPair(statementID, query);
                                    env.ImprovingPossibleIndices.TryAddPossibleIndices(improvingVirtualIndices, normalizedStatement, query);
                                    if (env.PossibleIndices.AllCoveringPerQuery.ContainsKey(queryKey))
                                    {
                                        env.ImprovingPossibleIndices.TryAddPossibleCoveringIndices(env.PossibleIndices.AllCoveringPerQuery[queryKey].Intersect(improvingVirtualIndices), normalizedStatement, query);
                                    }
                                }
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