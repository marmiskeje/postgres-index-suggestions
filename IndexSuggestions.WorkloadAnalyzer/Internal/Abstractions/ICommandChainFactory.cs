using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface ICommandChainFactory
    {
        IExecutableCommand WorkloadAnalysisChain(WorkloadAnalysisContext context);
    }
}
