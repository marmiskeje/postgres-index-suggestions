using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class EvaluateHPartitioningEnvironmentsCommand : ChainableCommand
    {
        private const decimal MIN_GLOBAL_IMPROVEMENT_RATIO = 0.25m; //25%
        private readonly ILog log;
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualHPartitioningsRepository virtualHPartitioningsRepository;
        private readonly IExplainRepository explainRepository;
        private readonly IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;
        public EvaluateHPartitioningEnvironmentsCommand(ILog log, WorkloadAnalysisContext context, IVirtualHPartitioningsRepository virtualHPartitioningsRepository,
                                                        IExplainRepository explainRepository, IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.log = log;
            this.context = context;
            this.virtualHPartitioningsRepository = virtualHPartitioningsRepository;
            this.explainRepository = explainRepository;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
        }
        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var env in context.HPartitioningDesignData.Environments)
                {
                    try
                    {
                        virtualHPartitioningsRepository.DestroyAll();
                        bool hPartitioningCreated = false;
                        try
                        {
                            var targetRelationData = context.RelationsData.GetReplacementOrOriginal(env.Partitioning.Relation.ID);
                            virtualHPartitioningsRepository.Create(dbObjectDefinitionGenerator.Generate(env.Partitioning.WithReplacedRelation(targetRelationData)));
                            hPartitioningCreated = true;
                        }
                        catch (Exception ex)
                        {
                            log.Write(ex);
                        }
                        if (hPartitioningCreated)
                        {
                            foreach (var queryPair in context.StatementsData.AllQueriesByRelation[env.Partitioning.Relation.ID])
                            {
                                var statementID = queryPair.NormalizedStatementID;
                                if (!env.StatementsEvaluation.ContainsKey(statementID))
                                {
                                    var workloadStatement = context.StatementsData.All[statementID];
                                    var normalizedStatement = workloadStatement.NormalizedStatement;
                                    var representativeStatement = workloadStatement.RepresentativeStatistics.RepresentativeStatement;
                                    var statementToUse = RepresentativeStatementReplacementUtility.Provide(normalizedStatement, representativeStatement, context.RelationsData);
                                    var realPlan = context.RealExecutionPlansForStatements[statementID].Plan;
                                    IExplainResult explainResult = context.RealExecutionPlansForStatements[statementID];
                                    var latestPlan = realPlan;
                                    if (normalizedStatement.CommandType == DAL.Contracts.StatementQueryCommandType.Select
                                        || normalizedStatement.CommandType == DAL.Contracts.StatementQueryCommandType.Insert) // hypo pg currently does not support explain for delete/update for h partitioning
                                    {
                                        explainResult = explainRepository.Eplain(statementToUse);
                                        latestPlan = explainResult.Plan; 
                                    }
                                    
                                    VirtualEnvironmentStatementEvaluation statementEvaluation = new VirtualEnvironmentStatementEvaluation();
                                    statementEvaluation.ExecutionPlan = explainResult;
                                    decimal fromPrice = realPlan.TotalCost;
                                    decimal toPrice = latestPlan.TotalCost;
                                    decimal divisor = Math.Abs(Math.Max(fromPrice, toPrice));
                                    if (divisor > 0)
                                    {
                                        statementEvaluation.LocalImprovementRatio = ((toPrice - fromPrice) / divisor) * -1m;
                                    }
                                    decimal statementPortion = context.StatementsData.AllExecutionsCount > 0 ? workloadStatement.TotalExecutionsCount / (decimal)context.StatementsData.AllExecutionsCount : 0m;
                                    statementEvaluation.GlobalImprovementRatio = statementEvaluation.LocalImprovementRatio * statementPortion;
                                    env.StatementsEvaluation.Add(statementID, statementEvaluation);

                                    env.Evaluation.ImprovementRatio += statementEvaluation.GlobalImprovementRatio;
                                }
                            }
                            env.IsImproving = env.Evaluation.ImprovementRatio >= MIN_GLOBAL_IMPROVEMENT_RATIO;
                        }
                    }
                    finally
                    {
                        virtualHPartitioningsRepository.DestroyAll();
                    }
                }
            }
        }
    }
}
