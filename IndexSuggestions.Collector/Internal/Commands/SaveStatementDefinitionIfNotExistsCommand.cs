using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class SaveStatementDefinitionIfNotExistsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public SaveStatementDefinitionIfNotExistsCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            if (context.PersistedData.NormalizedStatement.StatementDefinition == null)
            {
                StatementDefinition definition = new StatementDefinition();
                definition.CommandType = Convert(context.QueryTree.QueryCommandType);
                context.PersistedData.NormalizedStatement.StatementDefinition = definition;
                var statements = repositories.GetNormalizedStatementsRepository();
                statements.Update(context.PersistedData.NormalizedStatement);
            }
        }

        private StatementCommandType Convert(QueryCommandType source)
        {
            return (StatementCommandType)((int)source);
        }
    }
}
