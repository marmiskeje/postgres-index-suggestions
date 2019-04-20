using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
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
            context.IndicesDesignData.IndicesEnvironments.Clear();
            var possibleIndices = context.IndicesDesignData.PossibleIndices.Clone();
            context.IndicesDesignData.IndicesEnvironments.Add(new VirtualIndicesEnvironment(possibleIndices));
        }
    }
}
