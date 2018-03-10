using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface ILogEntryProcessingCommandFactory
    {
        IChainableCommand LoadQueryPlanToContextCommand(LogEntryProcessingContext context);
    }
}
