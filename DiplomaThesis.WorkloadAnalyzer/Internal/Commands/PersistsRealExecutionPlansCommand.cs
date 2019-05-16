using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class PersistsRealExecutionPlansCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRepositoriesFactory dalRepositories;
        public PersistsRealExecutionPlansCommand(WorkloadAnalysisContext context, IRepositoriesFactory dalRepositories)
        {
            this.context = context;
            this.dalRepositories = dalRepositories;
        }
        protected override void OnExecute()
        {
            var executionPlansRepository = dalRepositories.GetExecutionPlansRepository();
            var analysisRealStatementEvaluationsRepository = dalRepositories.GetWorkloadAnalysisRealStatementEvaluationsRepository();
            foreach (var kv in context.RealExecutionPlansForStatements)
            {
                var statementID = kv.Key;
                var explainResult = kv.Value;
                var createdPlan = Convert(statementID, explainResult);
                executionPlansRepository.Create(createdPlan);
                analysisRealStatementEvaluationsRepository.Create(Convert(context.WorkloadAnalysis.ID, statementID, createdPlan));
            }
        }

        private ExecutionPlan Convert(long statementID, DBMS.Contracts.IExplainResult explainResult)
        {
            return new ExecutionPlan() { Json = explainResult.PlanJson, TotalCost = explainResult.Plan.TotalCost };
        }

        private WorkloadAnalysisRealStatementEvaluation Convert(long workloadAnalysisID, long statementID, ExecutionPlan createdPlan)
        {
            return new WorkloadAnalysisRealStatementEvaluation() { ExecutionPlanID = createdPlan.ID, NormalizedStatementID = statementID, WorkloadAnalysisID = workloadAnalysisID };
        }
    }
}
