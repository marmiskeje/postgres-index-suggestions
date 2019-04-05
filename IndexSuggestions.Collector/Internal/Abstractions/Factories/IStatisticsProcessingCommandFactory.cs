using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.Collector
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
