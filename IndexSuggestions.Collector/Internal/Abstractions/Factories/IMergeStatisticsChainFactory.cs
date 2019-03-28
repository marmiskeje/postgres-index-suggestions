using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IMergeStatisticsChainFactory
    {
        IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementStatisticsContext context);
    }
}
