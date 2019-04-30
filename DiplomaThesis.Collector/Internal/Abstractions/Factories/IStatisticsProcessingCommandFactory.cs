using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;

namespace DiplomaThesis.Collector
{
    internal interface IStatisticsProcessingCommandFactory
    {
        IChainableCommand LoadDatabasesForTotalStatisticsCommand(TotalStatisticsCollectNextSampleContext context);
        IChainableCommand CollectTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context);
        IChainableCommand PublishTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context);
        IChainableCommand PersistDataAccumulatorsCommand();
        IChainableCommand PublishTotalViewStatisticsCommand(IStatementProcessingContext context);
        IChainableCommand EnqueueCommand();
    }
}
