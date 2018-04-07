using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class CreateOrUpdateNormalizedWorkloadStatementCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public CreateOrUpdateNormalizedWorkloadStatementCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            var repository = repositories.GetNormalizedWorkloadStatementsRepository();
            var entity = repository.Get(context.PersistedData.NormalizedStatement.ID, context.PersistedData.Workload.ID, true);
            if (entity == null)
            {
                entity = new NormalizedWorkloadStatement() { NormalizedStatementID = context.PersistedData.NormalizedStatement.ID, WorkloadID = context.PersistedData.Workload.ID, ExecutionsCount = 0 };
                repository.Create(entity);
            }
            entity.ExecutionsCount += 1;
            repository.Update(entity);
        }
    }
}
