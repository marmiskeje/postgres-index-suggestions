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
        private readonly IGeneralProcessingCommandFactory generalCommands;
        private readonly ILogEntryProcessingCommandFactory externalCommands;
        private readonly IRepositoriesFactory repositoriesFactory;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        public LogEntryProcessingChainFactory(ILog log, IGeneralProcessingCommandFactory generalCommands,
                                              ILogEntryProcessingCommandFactory externalCommands, IRepositoriesFactory repositoriesFactory,
                                              IStatementsProcessingDataAccumulator statementDataAccumulator)
        {
            this.log = log;
            this.generalCommands = generalCommands;
            this.externalCommands = externalCommands;
            this.repositoriesFactory = repositoriesFactory;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        /*
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
        }*/

        private IChainableCommand CreateStatementProcessingChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            // if enabled for database
            chain.Add(new ActionCommand(() =>
            {
                generalCommands.ExternalStatementNormalizationCommand(context).Execute();
                if (string.IsNullOrEmpty(context.StatementData.NormalizedStatement))
                {
                    externalCommands.NormalizeStatementCommand(log, context).Execute();
                }
                return !string.IsNullOrEmpty(context.StatementData.NormalizedStatement);
            }));
            chain.Add(generalCommands.ComputeNormalizedStatementFingerprintCommand(context));
            chain.Add(generalCommands.PublishNormalizedStatementCommand(context));
            chain.Add(generalCommands.PublishNormalizedStatementStatisticsCommand(context));
            
            ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();

            CommandChainCreator queryTreeProcessingChain = new CommandChainCreator();
            queryTreeProcessingChain.Add(new ActionCommand(() =>
            {
                var s = statementDataAccumulator.ProvideNormalizedStatement(context.StatementData.NormalizedStatementFingerprint);
                return s.StatementDefinition == null;
            }));
            queryTreeProcessingChain.Add(externalCommands.LoadQueryTreeToContextCommand(context));
            queryTreeProcessingChain.Add(generalCommands.PublishNormalizedStatementDefinitionCommand(context));
            parallelSteps.AddParallelStep(queryTreeProcessingChain.AsChainableCommand());

            CommandChainCreator queryPlanProcessingChain = new CommandChainCreator();
            queryPlanProcessingChain.Add(externalCommands.LoadQueryPlanToContextCommand(context));
            queryPlanProcessingChain.Add(generalCommands.PublishNormalizedStatementIndexStatisticsCommand(context));
            queryPlanProcessingChain.Add(generalCommands.PublishNormalizedStatementRelationStatisticsCommand(context));
            parallelSteps.AddParallelStep(queryPlanProcessingChain.AsChainableCommand());

            chain.Add(parallelSteps.CreateParallelCommand());
            return chain.AsChainableCommand();
        }

        public IExecutableCommand LogEntryProcessingChain(LogEntryProcessingContext context)
        {
            ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();
            parallelSteps.AddParallelStep(CreateStatementProcessingChain(context));

            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(generalCommands.LoadDatabaseInfoForLogEntryCommand(context));
            chain.Add(parallelSteps.CreateParallelCommand());
            /*
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
            */

            return chain.FirstCommand;
        }

        public IExecutableCommand LogEntryPersistenceChain()
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(generalCommands.PersistDataAccumulatorsCommand());
            return chain.FirstCommand;
        }
    }
}
