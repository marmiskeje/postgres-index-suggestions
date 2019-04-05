using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IMergeStatisticsChainFactory
    {
        IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementRelationStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementIndexStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeTotalRelationStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeTotalIndexStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeTotalStoredProcedureStatisticsContext context);
        IExecutableCommand MergeStatisticsChain(MergeTotalViewStatisticsContext context);
    }
}
