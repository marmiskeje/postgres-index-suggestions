using System;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class LoadWorkloadStatementsCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        private readonly INormalizedWorkloadStatementsRepository repository;

        public LoadWorkloadStatementsCommand(DesignIndicesContext context, INormalizedWorkloadStatementsRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        protected override void OnExecute()
        {
            var items = repository.GetWorkloadStatements(context.Workload, context.DataFilterFromDate, context.DataFilterToDate);
            foreach (var item in items)
            {
                context.Statements.Add(item.NormalizedStatement.ID, item);
            }
        }
    }
}