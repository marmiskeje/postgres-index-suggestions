using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class PublishNormalizedStatementCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementCommand(LogEntryProcessingContext context, IStatementDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            statementDataAccumulator.PublishNormalizedStatement(context.StatementData);
        }
    }
}
