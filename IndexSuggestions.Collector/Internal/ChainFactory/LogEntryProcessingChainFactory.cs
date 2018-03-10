using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface ILogEntryProcessingChainFactory
    {
        IExecutableCommand StatementIndexStatsChain(LogEntryProcessingContext context);
    }
    class LogEntryProcessingChainFactory : ILogEntryProcessingChainFactory
    {
        private readonly ILogEntryProcessingCommandFactory commandFactory;
        private readonly IRepositoriesFactory repositoriesFactory;
        public LogEntryProcessingChainFactory(ILogEntryProcessingCommandFactory commandFactory, IRepositoriesFactory repositoriesFactory)
        {
            this.commandFactory = commandFactory;
            this.repositoriesFactory = repositoriesFactory;
        }
        public IExecutableCommand StatementIndexStatsChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            // ignore own log entries!!!
            chain.Add(commandFactory.LoadQueryPlanToContextCommand(context));
            chain.Add(new NormalizeStatementCommand(context));
            chain.Add(new SaveStatementIndexUsageCommand(context, repositoriesFactory));
            return chain.FirstCommand;
        }
    }
}
