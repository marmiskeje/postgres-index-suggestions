using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class LoadQueryTreeToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingWrapperContext context;
        private readonly IRepositoriesFactory repositories;
        public LoadQueryTreeToContextCommand(LogEntryProcessingWrapperContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            if (context.QueryTree != null)
            {
                context.InnerContext.QueryTree = new QueryTreeProvider().Provide(context.QueryTree);
            }
        }
    }
}
