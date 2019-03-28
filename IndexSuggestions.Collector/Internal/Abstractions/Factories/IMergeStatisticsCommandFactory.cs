using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.Collector
{
    internal interface IMergeStatisticsCommandFactory
    {
        IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
    }
}
