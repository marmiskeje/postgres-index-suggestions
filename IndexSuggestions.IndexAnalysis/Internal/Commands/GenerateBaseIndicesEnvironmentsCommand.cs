using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    /// <summary>
    /// One environment with all possible indices
    /// </summary>
    internal class GenerateBaseIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        public GenerateBaseIndicesEnvironmentsCommand(DesignIndicesContext context)
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
