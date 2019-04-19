using System;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class LoadWorkloadCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        private readonly IWorkloadsRepository workloadsRepository;

        public LoadWorkloadCommand(DesignIndicesContext context, IWorkloadsRepository workloadsRepository)
        {
            this.context = context;
            this.workloadsRepository = workloadsRepository;
        }

        protected override void OnExecute()
        {
            context.Workload = workloadsRepository.GetByPrimaryKey(context.WorkloadID);
        }
    }
}