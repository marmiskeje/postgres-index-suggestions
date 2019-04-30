using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    /// <summary>
    /// One environment with all possible indices
    /// </summary>
    internal class GenerateBaseIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public GenerateBaseIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.Environments.Clear();
            var possibleIndices = context.IndicesDesignData.PossibleIndices.Clone();
            context.IndicesDesignData.Environments.Add(new VirtualIndicesEnvironment(possibleIndices));
        }
    }
}
