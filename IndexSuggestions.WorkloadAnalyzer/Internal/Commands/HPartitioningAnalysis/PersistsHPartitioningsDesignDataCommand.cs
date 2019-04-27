using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class PersistsHPartitioningsDesignDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly ISqlCreateStatementGenerator sqlCreateStatementGenerator;
        public PersistsHPartitioningsDesignDataCommand(WorkloadAnalysisContext context, IRepositoriesFactory dalRepositories, ISqlCreateStatementGenerator sqlCreateStatementGenerator)
        {
            this.context = context;
            this.dalRepositories = dalRepositories;
            this.sqlCreateStatementGenerator = sqlCreateStatementGenerator;
        }
        protected override void OnExecute()
        {
            var virtualEnvsRepository = dalRepositories.GetVirtualEnvironmentsRepository();
            var virtualHPartitioningsRepository = dalRepositories.GetVirtualEnvironmentPossibleHPartitioningsRepository();
            var executionPlansRepository = dalRepositories.GetExecutionPlansRepository();
            var virtualEnvStatementEvalsRepository = dalRepositories.GetVirtualEnvironmentStatementEvaluationsRepository();
            var designData = context.HPartitioningDesignData;
            using (var scope = new TransactionScope())
            {
                foreach (var env in designData.Environments)
                {
                    var createdEnvironment = Convert(env);
                    virtualEnvsRepository.Create(createdEnvironment);
                    var createdHPartitioning = Convert(createdEnvironment, env.Partitioning);
                    virtualHPartitioningsRepository.Create(createdHPartitioning);

                    foreach (var kv in env.PlansPerStatement)
                    {
                        var statementID = kv.Key;
                        var explainResult = kv.Value;
                        var createdPlan = Convert(statementID, explainResult);
                        executionPlansRepository.Create(createdPlan);
                        virtualEnvStatementEvalsRepository.Create(Convert(createdEnvironment, statementID, createdPlan));
                    }
                }
                scope.Complete();
            }
        }
        private VirtualEnvironmentStatementEvaluation Convert(VirtualEnvironment env, long statementID, ExecutionPlan plan)
        {
            VirtualEnvironmentStatementEvaluation result = new VirtualEnvironmentStatementEvaluation();
            result.ExecutionPlanID = plan.ID;
            result.NormalizedStatementID = statementID;
            result.VirtualEnvironmentID = env.ID;
            return result;
        }
        private ExecutionPlan Convert(long statementID, DBMS.Contracts.IExplainResult explainResult)
        {
            return new ExecutionPlan() { Json = explainResult.PlanJson, TotalCost = explainResult.Plan.TotalCost };
        }

        private VirtualEnvironmentPossibleHPartitioning Convert(VirtualEnvironment environment, HPartitioningDefinition partitioning)
        {
            VirtualEnvironmentPossibleHPartitioning result = new VirtualEnvironmentPossibleHPartitioning();
            var definition = sqlCreateStatementGenerator.Generate(partitioning);
            result.PartitioningStatement = definition.PartitioningStatement;
            result.PartitionStatements = definition.PartitionStatements;
            result.RelationID = partitioning.Relation.ID;
            result.VirtualEnvironmentID = environment.ID;
            return result;
        }

        private VirtualEnvironment Convert(VirtualHPartitioningEnvironment env)
        {
            VirtualEnvironment result = new VirtualEnvironment();
            result.WorkloadAnalysisID = context.WorkloadAnalysis.ID;
            result.Type = VirtualEnvironmentType.HPartitionings;
            return result;
        }
    }
}
