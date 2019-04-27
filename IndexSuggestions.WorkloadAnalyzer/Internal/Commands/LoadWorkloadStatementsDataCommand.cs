using System;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class LoadWorkloadStatementsDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly INormalizedWorkloadStatementsRepository repository;

        public LoadWorkloadStatementsDataCommand(WorkloadAnalysisContext context, INormalizedWorkloadStatementsRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        protected override void OnExecute()
        {
            var items = repository.GetWorkloadStatements(context.Workload, context.WorkloadAnalysis.PeriodFromDate, context.WorkloadAnalysis.PeriodToDate);
            context.StatementsData = new WorkloadStatementsData(items);
        }
    }
}