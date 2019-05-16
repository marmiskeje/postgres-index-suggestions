using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class LoadQueryTreesToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingWrapperContext context;
        private readonly IRepositoriesFactory repositories;
        public LoadQueryTreesToContextCommand(LogEntryProcessingWrapperContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            foreach (var item in context.QueryTrees)
            {
                context.InnerContext.QueryTrees.Add(new QueryTreeProvider().Provide(item));
            }
        }
    }
}
