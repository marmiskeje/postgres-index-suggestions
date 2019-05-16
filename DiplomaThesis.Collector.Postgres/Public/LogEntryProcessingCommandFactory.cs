using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
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
            chain.Add(new LoadDebugTreesToContextCommand(() => context.Entry.QueryTrees, () => wrapperContext.QueryTrees));
            chain.Add(new LoadQueryTreesToContextCommand(wrapperContext, repositories));
            return chain.AsChainableCommand();
        }

        public IChainableCommand LoadQueryPlanToContextCommand(LogEntryProcessingContext context)
        {
            LogEntryProcessingWrapperContext wrapperContext = new LogEntryProcessingWrapperContext();
            wrapperContext.InnerContext = context;
            var chain = new CommandChainCreator();
            chain.Add(new LoadDebugTreesToContextCommand(() => context.Entry.PlanTrees, () => wrapperContext.QueryPlans));
            chain.Add(new LoadQueryPlansToContextCommand(wrapperContext));
            return chain.AsChainableCommand();
        }

        public IChainableCommand NormalizeStatementCommand(ILog log, LogEntryProcessingContext context)
        {
            return new NormalizeStatementCommand(log, context);
        }
    }
}
