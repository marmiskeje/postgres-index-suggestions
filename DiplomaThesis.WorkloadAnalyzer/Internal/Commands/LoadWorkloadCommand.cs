using System;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class LoadWorkloadCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IWorkloadsRepository workloadsRepository;

        public LoadWorkloadCommand(WorkloadAnalysisContext context, IWorkloadsRepository workloadsRepository)
        {
            this.context = context;
            this.workloadsRepository = workloadsRepository;
        }

        protected override void OnExecute()
        {
            context.Workload = workloadsRepository.GetByPrimaryKey(context.WorkloadAnalysis.WorkloadID);
        }
    }
}