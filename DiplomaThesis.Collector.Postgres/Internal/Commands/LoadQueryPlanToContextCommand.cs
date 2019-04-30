using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class LoadQueryPlanToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingWrapperContext context;
        public LoadQueryPlanToContextCommand(LogEntryProcessingWrapperContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            if (context.QueryPlan != null)
            {
                context.InnerContext.QueryPlan = new QueryPlanProvider().Provide(context.QueryPlan);
            }
        }
    }
}
