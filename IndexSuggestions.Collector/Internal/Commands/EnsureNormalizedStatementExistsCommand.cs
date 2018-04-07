using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class EnsureNormalizedStatementExistsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public EnsureNormalizedStatementExistsCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            var statements = repositories.GetNormalizedStatementsRepository();
            var statement = statements.GetByStatementFingerprint(context.NormalizedStatementFingerprint, true);
            if (statement == null)
            {
                statement = new NormalizedStatement() { Statement = context.NormalizedStatement, StatementFingerprint = context.NormalizedStatementFingerprint };
                statements.Create(statement);
            }
            context.PersistedData.NormalizedStatement = statement;
        }
    }
}
