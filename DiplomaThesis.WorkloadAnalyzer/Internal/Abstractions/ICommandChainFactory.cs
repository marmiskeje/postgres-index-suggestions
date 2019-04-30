using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal interface ICommandChainFactory
    {
        IExecutableCommand WorkloadAnalysisChain(WorkloadAnalysisContext context);
    }
}
