using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class PersistsHPartitioningsDesignDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;
        public PersistsHPartitioningsDesignDataCommand(WorkloadAnalysisContext context, IRepositoriesFactory dalRepositories,
                                                       DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.context = context;
            this.dalRepositories = dalRepositories;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
        }
        protected override void OnExecute()
        {
            var virtualEnvsRepository = dalRepositories.GetVirtualEnvironmentsRepository();
            var virtualHPartitioningsRepository = dalRepositories.GetVirtualEnvironmentPossibleHPartitioningsRepository();
            var executionPlansRepository = dalRepositories.GetExecutionPlansRepository();
            var virtualEnvStatementEvalsRepository = dalRepositories.GetVirtualEnvironmentStatementEvaluationsRepository();
            var designData = context.HPartitioningDesignData;
            foreach (var env in designData.Environments)
            {
                var createdEnvironment = Convert(env);
                virtualEnvsRepository.Create(createdEnvironment);
                var createdHPartitioning = Convert(createdEnvironment, env.Partitioning, env.Evaluation);
                virtualHPartitioningsRepository.Create(createdHPartitioning);

                foreach (var kv in env.StatementsEvaluation)
                {
                    var statementID = kv.Key;
                    var eval = kv.Value;
                    var createdPlan = Convert(statementID, eval.ExecutionPlan);
                    executionPlansRepository.Create(createdPlan);
                    virtualEnvStatementEvalsRepository.Create(Convert(createdEnvironment, statementID, createdPlan, eval));
                }
            }
        }
        private DAL.Contracts.VirtualEnvironmentStatementEvaluation Convert(VirtualEnvironment env, long statementID, ExecutionPlan plan, VirtualEnvironmentStatementEvaluation eval)
        {
            DAL.Contracts.VirtualEnvironmentStatementEvaluation result = new DAL.Contracts.VirtualEnvironmentStatementEvaluation();
            result.ExecutionPlanID = plan.ID;
            result.NormalizedStatementID = statementID;
            result.VirtualEnvironmentID = env.ID;
            result.GlobalImprovementRatio = eval.GlobalImprovementRatio;
            result.LocalImprovementRatio = eval.LocalImprovementRatio;
            return result;
        }
        private ExecutionPlan Convert(long statementID, DBMS.Contracts.IExplainResult explainResult)
        {
            return new ExecutionPlan() { Json = explainResult.PlanJson, TotalCost = explainResult.Plan.TotalCost };
        }

        private VirtualEnvironmentPossibleHPartitioning Convert(VirtualEnvironment environment, DBMS.Contracts.HPartitioningDefinition partitioning, VirtualHPartitioningEnvironmentHPartitioningEvaluation eval)
        {
            VirtualEnvironmentPossibleHPartitioning result = new VirtualEnvironmentPossibleHPartitioning();
            var definition = dbObjectDefinitionGenerator.Generate(partitioning);
            result.PartitioningStatement = definition.PartitioningStatement;
            result.PartitionStatements = definition.PartitionStatements;
            result.RelationID = partitioning.Relation.ID;
            result.VirtualEnvironmentID = environment.ID;
            result.ImprovementRatio = eval.ImprovementRatio;
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
