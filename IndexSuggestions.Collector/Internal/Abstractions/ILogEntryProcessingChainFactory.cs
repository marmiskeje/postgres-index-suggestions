using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface ILogEntryProcessingChainFactory
    {
        IExecutableCommand StatementIndexStatsChain(LogEntryProcessingContext context);
    }
}
