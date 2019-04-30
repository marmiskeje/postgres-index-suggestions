using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class ExcludeExistingIndicesCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public ExcludeExistingIndicesCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.PossibleIndices.Remove(context.IndicesDesignData.ExistingIndices.All);
        }
    }
}
