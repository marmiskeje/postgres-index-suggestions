using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    public class LogEntryProcessingCommandFactory : ILogEntryProcessingCommandFactory
    {
        public IChainableCommand LoadQueryPlanToContextCommand(LogEntryProcessingContext context)
        {
            LogEntryProcessingWrapperContext wrapperContext = new LogEntryProcessingWrapperContext();
            wrapperContext.InnerContext = context;
            var chain = new CommandChainCreator();
            chain.Add(new LoadDebugTreeToContextCommand(() => context.Entry.PlanTree, x => wrapperContext.QueryPlan = x));
            chain.Add(new LoadQueryPlanToContextCommand(wrapperContext));
            return chain.AsChainableCommand();
        }

        public IChainableCommand NormalizeStatementCommand(ILog log, LogEntryProcessingContext context)
        {
            return new NormalizeStatementCommand(log, context);
        }
    }
}
