using System;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class LoadWorkloadStatementsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly INormalizedWorkloadStatementsRepository repository;

        public LoadWorkloadStatementsCommand(WorkloadAnalysisContext context, INormalizedWorkloadStatementsRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        protected override void OnExecute()
        {
            var items = repository.GetWorkloadStatements(context.Workload, context.WorkloadAnalysis.PeriodFromDate, context.WorkloadAnalysis.PeriodToDate);
            foreach (var item in items)
            {
                context.Statements.Add(item.NormalizedStatement.ID, item);
            }
        }
    }
}