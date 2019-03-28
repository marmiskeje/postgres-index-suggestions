using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.Collector
{
    internal interface IMergeStatisticsCommandFactory
    {
        IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementRelationStatisticsContext context);
        IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementIndexStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context);
    }
}
