using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class ExcludeExistingIndicesCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        public ExcludeExistingIndicesCommand(DesignIndicesContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.PossibleIndices.Remove(context.ExistingIndices);
        }
    }
}
