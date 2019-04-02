using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IStatisticsProcessingChainFactory
    {
        IExecutableCommand TotalStatisticsCollectNextSampleChain(TotalStatisticsCollectNextSampleContext context);
        IExecutableCommand TotalStatisticsPersistenceChain();
    }
}
