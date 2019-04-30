using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class CleanUpNotImprovingHPartitioningAndTheirEnvsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public CleanUpNotImprovingHPartitioningAndTheirEnvsCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.HPartitioningDesignData.Environments.RemoveAll(x => !x.IsImproving);
        }
    }
}
