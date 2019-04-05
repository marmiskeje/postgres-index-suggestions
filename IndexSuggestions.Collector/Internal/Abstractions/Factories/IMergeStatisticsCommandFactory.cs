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
        IChainableCommand LoadStatisticsForMergeCommand(MergeTotalRelationStatisticsContext context);
        IChainableCommand LoadStatisticsForMergeCommand(MergeTotalIndexStatisticsContext context);
        IChainableCommand LoadStatisticsForMergeCommand(MergeTotalStoredProcedureStatisticsContext context);
        IChainableCommand LoadStatisticsForMergeCommand(MergeTotalViewStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeTotalRelationStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeTotalIndexStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeTotalStoredProcedureStatisticsContext context);
        IChainableCommand MergeStatisticsCommand(MergeTotalViewStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeTotalRelationStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeTotalIndexStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeTotalStoredProcedureStatisticsContext context);
        IChainableCommand SaveMergedStatisticsCommand(MergeTotalViewStatisticsContext context);
    }
}
