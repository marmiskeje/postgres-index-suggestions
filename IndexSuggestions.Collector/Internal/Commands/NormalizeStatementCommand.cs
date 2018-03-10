using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class NormalizeStatementCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        public NormalizeStatementCommand(LogEntryProcessingContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            this.context.NormalizedStatement = context.Entry.Statement;
        }
    }
}
