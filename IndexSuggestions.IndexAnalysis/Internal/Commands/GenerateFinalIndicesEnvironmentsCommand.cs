using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    /// <summary>
    /// Generates combinations of indices
    /// </summary>
    internal class GenerateFinalIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        public GenerateFinalIndicesEnvironmentsCommand(DesignIndicesContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.IndicesEnvironments.Clear();
            var allIndices = new ElementSet<IndexDefinition>(context.IndicesDesignData.PossibleIndices.All);
            for (int combinationLength = 1; combinationLength < allIndices.Count; combinationLength++)
            {
                var combinations = new Combinations<IndexDefinition>(allIndices, combinationLength);
                foreach (var c in combinations)
                {
                    context.IndicesDesignData.IndicesEnvironments.Add(new VirtualIndicesEnvironment(context.IndicesDesignData.PossibleIndices.ToSubSetOf(c)));
                }
            }
        }
    }
}
