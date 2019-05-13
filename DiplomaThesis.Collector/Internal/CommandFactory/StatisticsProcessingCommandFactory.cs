using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.Collector
{
    internal class StatisticsProcessingCommandFactory : IStatisticsProcessingCommandFactory
    {
        private readonly ILog log;
        private readonly ICommandProcessingQueue<IExecutableCommand> queue;
        private readonly IStatisticsProcessingDataAccumulator statisticsAccumulator;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly IDatabaseDependencyHierarchyProvider dependencyHierarchyProvider;
        public StatisticsProcessingCommandFactory(ILog log, ICommandProcessingQueue<IExecutableCommand> queue, IStatisticsProcessingDataAccumulator statisticsAccumulator,
                                                  DBMS.Contracts.IRepositoriesFactory dbmsRepositories, IRepositoriesFactory dalRepositories,
                                                  IDatabaseDependencyHierarchyProvider dependencyHierarchyProvider)
        {
            this.log = log;
            this.queue = queue;
            this.statisticsAccumulator = statisticsAccumulator;
            this.dbmsRepositories = dbmsRepositories;
            this.dalRepositories = dalRepositories;
            this.dependencyHierarchyProvider = dependencyHierarchyProvider;
        }
        public IChainableCommand CollectTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new CollectTotalDatabaseStatisticsCommand(log, context, dbmsRepositories.GetTotalDatabaseStatisticsRepository());
        }

        public IChainableCommand EnqueueCommand()
        {
            return new EnqueueCommand(queue);
        }

        public IChainableCommand LoadDatabasesForTotalStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new LoadDatabasesForTotalStatisticsCommand(context, dbmsRepositories.GetDatabasesRepository(), dalRepositories.GetSettingPropertiesRepository());
        }

        public IChainableCommand PersistDataAccumulatorsCommand()
        {
            return new PersistStatisticsDataAccumulatorsCommand(statisticsAccumulator, dalRepositories);
        }

        public IChainableCommand PublishTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new PublishTotalDatabaseStatisticsCommand(context, statisticsAccumulator);
        }

        public IChainableCommand PublishTotalViewStatisticsCommand(IStatementProcessingContext context)
        {
            return new PublishTotalViewStatisticsCommand(context, dependencyHierarchyProvider, statisticsAccumulator);
        }
    }
}
