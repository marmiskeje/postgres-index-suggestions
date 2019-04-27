﻿using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class InitializeHPartitioningEnvironmentCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IVirtualHPartitioningsRepository repository;

        public InitializeHPartitioningEnvironmentCommand(WorkloadAnalysisContext context, IVirtualHPartitioningsRepository repository)
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
