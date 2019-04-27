﻿using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class EvaluateHPartitioningEnvironmentsCommand : ChainableCommand
    {
        private const decimal MIN_COST_PERCENTAGE_IMPROVEMENT = 0.75m; //75%
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualHPartitioningsRepository virtualHPartitioningsRepository;
        private readonly IExplainRepository explainRepository;
        private readonly ISqlCreateStatementGenerator sqlCreateStatementGenerator;
        public EvaluateHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context, IVirtualHPartitioningsRepository virtualHPartitioningsRepository,
                                                        IExplainRepository explainRepository, ISqlCreateStatementGenerator sqlCreateStatementGenerator)
        {
            this.context = context;
            this.virtualHPartitioningsRepository = virtualHPartitioningsRepository;
            this.explainRepository = explainRepository;
            this.sqlCreateStatementGenerator = sqlCreateStatementGenerator;
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
                        var targetRelationData = context.RelationsData.GetReplacementOrOriginal(env.Partitioning.Relation.ID);
                        virtualHPartitioningsRepository.Create(sqlCreateStatementGenerator.Generate(env.Partitioning.WithReplacedRelation(targetRelationData)));
                        decimal latestWeightedTotalCost = 0;
                        decimal originalWeightedTotalCost = 0;
                        foreach (var queryPair in context.StatementsData.AllSelectQueriesByRelation[env.Partitioning.Relation.ID])
                        {
                            var statementID = queryPair.NormalizedStatementID;
                            if (!env.PlansPerStatement.ContainsKey(statementID))
                            {
                                var normalizedStatement = context.StatementsData.AllSelects[statementID].NormalizedStatement;
                                var representativeStatement = context.StatementsData.AllSelects[statementID].RepresentativeStatistics.RepresentativeStatement;
                                var statementToUse = RepresentativeStatementReplacementUtility.Provide(normalizedStatement, representativeStatement, context.RelationsData);
                                var explainResult = explainRepository.Eplain(statementToUse);
                                var latestPlan = explainResult.Plan;
                                env.PlansPerStatement.Add(statementID, explainResult);

                                decimal weight = context.StatementsData.All[statementID].TotalExecutionsCount;
                                latestWeightedTotalCost += weight * latestPlan.TotalCost;
                                originalWeightedTotalCost += weight * context.RealExecutionPlansForStatements[statementID].Plan.TotalCost;
                            }
                        }
                        env.IsImproving = latestWeightedTotalCost <= originalWeightedTotalCost * MIN_COST_PERCENTAGE_IMPROVEMENT;
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
