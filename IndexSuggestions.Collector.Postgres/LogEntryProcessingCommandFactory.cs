using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    public class LogEntryProcessingCommandFactory : ILogEntryProcessingCommandFactory
    {
        private readonly IRepositoriesFactory repositories;
        public LogEntryProcessingCommandFactory(IRepositoriesFactory repositories)
        {
            this.repositories = repositories;
        }
        public IChainableCommand LoadQueryTreeToContextCommand(LogEntryProcessingContext context)
        {
            LogEntryProcessingWrapperContext wrapperContext = new LogEntryProcessingWrapperContext();
            wrapperContext.InnerContext = context;
            var chain = new CommandChainCreator();
            chain.Add(new LoadDebugTreeToContextCommand(() => context.Entry.QueryTree, x => wrapperContext.QueryTree = x));
            chain.Add(new LoadQueryTreeToContextCommand(wrapperContext, repositories));
            return chain.AsChainableCommand();
        }

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
