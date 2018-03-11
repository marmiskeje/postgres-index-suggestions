using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface ILogEntryProcessingCommandFactory
    {
        IChainableCommand LoadQueryPlanToContextCommand(LogEntryProcessingContext context);
        IChainableCommand NormalizeStatementCommand(ILog log, LogEntryProcessingContext context);
    }
}
