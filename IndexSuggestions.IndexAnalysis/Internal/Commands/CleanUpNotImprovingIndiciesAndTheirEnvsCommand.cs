using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IndexSuggestions.Common;

namespace IndexSuggestions.IndexAnalysis
{
    /// <summary>
    /// Remove not improving indices and thei environments
    /// </summary>
    internal class CleanUpNotImprovingIndiciesAndTheirEnvsCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        public CleanUpNotImprovingIndiciesAndTheirEnvsCommand(DesignIndicesContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            var allImprovingIndices = new HashSet<IndexDefinition>();
            foreach (var env in context.IndicesDesignData.IndicesEnvironments)
            {
                allImprovingIndices.AddRange(env.ImprovingPossibleIndices.All);
            }
            var notImprovingIndices = context.IndicesDesignData.PossibleIndices.All.Intersect(allImprovingIndices);
            List<VirtualIndicesEnvironment> environmentsToDel = new List<VirtualIndicesEnvironment>();
            foreach (var env in context.IndicesDesignData.IndicesEnvironments)
            {
                if (env.ImprovingPossibleIndices.All.Count == 0 || env.PossibleIndices.All.Intersect(notImprovingIndices).Count() > 0)
                {
                    environmentsToDel.Add(env);
                }
            }
            environmentsToDel.ForEach(x => context.IndicesDesignData.IndicesEnvironments.Remove(x));
            context.IndicesDesignData.PossibleIndices.Remove(notImprovingIndices);
        }
    }
}
