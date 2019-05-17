using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    class LogEntryProcessingChainFactory : ILogEntryProcessingChainFactory
    {
        private readonly ILog log;
        private readonly IGeneralProcessingCommandFactory generalCommands;
        private readonly IStatisticsProcessingCommandFactory statisticsProcessingCommands;
        private readonly ILogEntryProcessingCommandFactory externalCommands;
        private readonly IRepositoriesFactory repositoriesFactory;
        public LogEntryProcessingChainFactory(ILog log, IGeneralProcessingCommandFactory generalCommands, IStatisticsProcessingCommandFactory statisticsProcessingCommands,
                                              ILogEntryProcessingCommandFactory externalCommands, IRepositoriesFactory repositoriesFactory)
        {
            this.log = log;
            this.generalCommands = generalCommands;
            this.statisticsProcessingCommands = statisticsProcessingCommands;
            this.externalCommands = externalCommands;
            this.repositoriesFactory = repositoriesFactory;
        }

        private IChainableCommand CreateStatementProcessingChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new ActionCommand(() => context.DatabaseCollectingConfiguration.IsEnabledStatementCollection));
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
            
            
            ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();

            CommandChainCreator queryTreeProcessingChain = new CommandChainCreator();
            queryTreeProcessingChain.Add(externalCommands.LoadQueryTreeToContextCommand(context));
            queryTreeProcessingChain.Add(generalCommands.PublishNormalizedStatementDefinitionCommand(context));
            parallelSteps.AddParallelStep(queryTreeProcessingChain.AsChainableCommand());

            CommandChainCreator queryPlanProcessingChain = new CommandChainCreator();
            queryPlanProcessingChain.Add(externalCommands.LoadQueryPlanToContextCommand(context));
            queryPlanProcessingChain.Add(generalCommands.PublishNormalizedStatementIndexStatisticsCommand(context));
            queryPlanProcessingChain.Add(generalCommands.PublishNormalizedStatementRelationStatisticsCommand(context));
            parallelSteps.AddParallelStep(queryPlanProcessingChain.AsChainableCommand());

            chain.Add(parallelSteps.CreateParallelCommand());
            chain.Add(generalCommands.PublishNormalizedStatementStatisticsCommand(context));
            return chain.AsChainableCommand();
        }

        private IChainableCommand CreateViewStatisticsProcessingChain(LogEntryProcessingContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(new ActionCommand(() => context.DatabaseCollectingConfiguration.IsEnabledGeneralCollection));
            chain.Add(statisticsProcessingCommands.EnqueueCommand());
            chain.Add(statisticsProcessingCommands.PublishTotalViewStatisticsCommand(context));
            return chain.AsChainableCommand();
        }

        public IExecutableCommand LogEntryProcessingChain(LogEntryProcessingContext context)
        {
            ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();
            parallelSteps.AddParallelStep(CreateStatementProcessingChain(context));
            parallelSteps.AddParallelStep(CreateViewStatisticsProcessingChain(context));

            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(generalCommands.LoadDatabaseInfoForLogEntryCommand(context));
            chain.Add(parallelSteps.CreateParallelCommand());

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
