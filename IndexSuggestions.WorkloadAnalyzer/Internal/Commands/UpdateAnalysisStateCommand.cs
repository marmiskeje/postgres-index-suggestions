using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class UpdateAnalysisStateCommand : ChainableCommand
    {
        private static readonly object lockObject = new object();
        private readonly WorkloadAnalysisContext context;
        private readonly WorkloadAnalysisStateType desiredState;
        private readonly IWorkloadAnalysesRepository repository;
        public UpdateAnalysisStateCommand(WorkloadAnalysisContext context, WorkloadAnalysisStateType desiredState, IWorkloadAnalysesRepository repository)
        {
            this.context = context;
            this.desiredState = desiredState;
            this.repository = repository;
        }
        protected override void OnExecute()
        {
            DateTime now = DateTime.Now;
            switch (desiredState)
            {
                case WorkloadAnalysisStateType.InProgress:
                    {
                        lock (lockObject)
                        {
                            context.WorkloadAnalysis = repository.GetByPrimaryKey(context.WorkloadAnalysis.ID);
                            if (context.WorkloadAnalysis.State == WorkloadAnalysisStateType.Created)
                            {
                                context.WorkloadAnalysis.State = WorkloadAnalysisStateType.InProgress;
                                context.WorkloadAnalysis.StartDate = now;
                                repository.Update(context.WorkloadAnalysis);
                            }
                            else
                            {
                                // already processed or processing
                                IsEnabledSuccessorCall = false;
                            }
                        }
                    }
                    break;
                case WorkloadAnalysisStateType.EndedSuccesfully:
                case WorkloadAnalysisStateType.EndedWithError:
                    {
                        context.WorkloadAnalysis.EndDate = now;
                        context.WorkloadAnalysis.State = desiredState;
                        repository.Update(context.WorkloadAnalysis);
                    }
                    break;
            }
        }
    }
}
