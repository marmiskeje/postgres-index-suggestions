﻿using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class InitializeVirtualIndicesEnvironmentCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualIndicesRepository repository;

        public InitializeVirtualIndicesEnvironmentCommand(WorkloadAnalysisContext context, IVirtualIndicesRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                repository.InitializeEnvironment();
            }
        }
    }
}