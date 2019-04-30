using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public interface ILogEntryProcessingCommandFactory
    {
        IChainableCommand LoadQueryPlanToContextCommand(LogEntryProcessingContext context);
        IChainableCommand LoadQueryTreeToContextCommand(LogEntryProcessingContext context);
        IChainableCommand NormalizeStatementCommand(ILog log, LogEntryProcessingContext context);
    }
}
