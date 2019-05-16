using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class LoadQueryPlansToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingWrapperContext context;
        public LoadQueryPlansToContextCommand(LogEntryProcessingWrapperContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            foreach (var item in context.QueryPlans)
            {
                context.InnerContext.QueryPlans.Add(new QueryPlanProvider().Provide(item));
            }
        }
    }
}
