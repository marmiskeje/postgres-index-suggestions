using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.Collector
{
    internal class StatisticsProcessingCommandFactory : IStatisticsProcessingCommandFactory
    {
        private readonly ILog log;
        private readonly IStatisticsProcessingDataAccumulator statisticsAccumulator;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        private readonly IRepositoriesFactory dalRepositories;
        public StatisticsProcessingCommandFactory(ILog log, IStatisticsProcessingDataAccumulator statisticsAccumulator, DBMS.Contracts.IRepositoriesFactory dbmsRepositories,
                                                  IRepositoriesFactory dalRepositories)
        {
            this.log = log;
            this.statisticsAccumulator = statisticsAccumulator;
            this.dbmsRepositories = dbmsRepositories;
            this.dalRepositories = dalRepositories;
        }
        public IChainableCommand CollectTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new CollectTotalDatabaseStatisticsCommand(log, context, dbmsRepositories.GetTotalDatabaseStatisticsRepository());
        }

        public IChainableCommand LoadDatabasesForTotalStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new LoadDatabasesForTotalStatisticsCommand(context, dbmsRepositories.GetDatabasesRepository());
        }

        public IChainableCommand PersistDataAccumulatorsCommand()
        {
            return new PersistStatisticsDataAccumulatorsCommand(statisticsAccumulator, dalRepositories);
        }

        public IChainableCommand PublishTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context)
        {
            return new PublishTotalDatabaseStatisticsCommand(context, statisticsAccumulator);
        }
    }
}
