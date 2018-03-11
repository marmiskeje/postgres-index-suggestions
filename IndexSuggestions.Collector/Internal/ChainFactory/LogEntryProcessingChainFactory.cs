using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    class LogEntryProcessingChainFactory : ILogEntryProcessingChainFactory
    {
        private readonly ILog log;
        private readonly ILogEntryProcessingCommandFactory commandFactory;
        private readonly IRepositoriesFactory repositoriesFactory;
        private readonly ILastProcessedLogEntryEvidence lastProcessedEvidence;
        public LogEntryProcessingChainFactory(ILog log, ILogEntryProcessingCommandFactory commandFactory, IRepositoriesFactory repositoriesFactory,
                                              ILastProcessedLogEntryEvidence lastProcessedEvidence)
        {
            this.log = log;
            this.commandFactory = commandFactory;
            this.repositoriesFactory = repositoriesFactory;
            this.lastProcessedEvidence = lastProcessedEvidence;
        }
        public IExecutableCommand StatementIndexStatsChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new HandleExceptionCommand(finallyAction: () => lastProcessedEvidence.Publish(context.Entry.Timestamp)));
            chain.Add(new IgnoreOwnLogEntriesCommand(context));
            chain.Add(commandFactory.LoadQueryPlanToContextCommand(context));
            chain.Add(commandFactory.NormalizeStatementCommand(log, context));
            chain.Add(new SaveStatementIndexUsageCommand(context, repositoriesFactory));
            return chain.FirstCommand;
        }
    }
}
