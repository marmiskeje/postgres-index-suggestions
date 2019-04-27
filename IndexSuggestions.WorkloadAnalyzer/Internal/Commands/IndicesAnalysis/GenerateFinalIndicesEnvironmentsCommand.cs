using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    /// <summary>
    /// Generates combinations of indices
    /// </summary>
    internal class GenerateFinalIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public GenerateFinalIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.Environments.Clear();
            var allIndices = new ElementSet<IndexDefinition>(context.IndicesDesignData.PossibleIndices.All);
            for (int combinationLength = 1; combinationLength < allIndices.Count; combinationLength++)
            {
                var combinations = new Combinations<IndexDefinition>(allIndices, combinationLength);
                foreach (var c in combinations)
                {
                    context.IndicesDesignData.Environments.Add(new VirtualIndicesEnvironment(context.IndicesDesignData.PossibleIndices.ToSubSetOf(c)));
                }
            }
        }
    }
}
