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

        private IChainableCommand CreateWorkloadProcessingChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new LoadWorkloadToContextCommand(context, repositoriesFactory));
            chain.Add(new ApplyLogEntryWorkloadDefinitionCommand(context));
            chain.Add(commandFactory.LoadQueryTreeToContextCommand(context));
            chain.Add(new ApplyQueryTreeWorkloadDefinitionCommand(context));
            chain.Add(new SaveStatementDefinitionIfNotExistsCommand(context, repositoriesFactory));
            chain.Add(new CreateOrUpdateNormalizedWorkloadStatementCommand(context, repositoriesFactory));
            return chain.AsChainableCommand();
        }

        private IChainableCommand CreateStatementIndexUsageStatsChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new SaveStatementIndexUsageCommand(context, repositoriesFactory));
            return chain.AsChainableCommand();
        }

        public IExecutableCommand StatementProcessingChain(LogEntryProcessingContext context)
        {
            ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();
            parallelSteps.AddParallelStep(CreateWorkloadProcessingChain(context));
            parallelSteps.AddParallelStep(CreateStatementIndexUsageStatsChain(context));

            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new HandleExceptionCommand(finallyAction: () => lastProcessedEvidence.Publish(context.Entry.Timestamp)));
            chain.Add(new IgnoreOwnLogEntriesCommand(context));
            chain.Add(commandFactory.NormalizeStatementCommand(log, context));
            chain.Add(new ComputeNormalizedStatementFingerprintCommand(context));
            chain.Add(new EnsureNormalizedStatementExistsCommand(context, repositoriesFactory));
            chain.Add(commandFactory.LoadQueryPlanToContextCommand(context));
            chain.Add(parallelSteps.CreateParallelCommand());


            return chain.FirstCommand;
        }
    }
}
