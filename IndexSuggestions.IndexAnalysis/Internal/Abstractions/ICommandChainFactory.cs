using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal interface ICommandChainFactory
    {
        IExecutableCommand DesignIndicesChain(DesignIndicesContext context);
    }
}
