using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal class LoadQueryTreeToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingWrapperContext context;
        public LoadQueryTreeToContextCommand(LogEntryProcessingWrapperContext context)
        {
            this.context = context;
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
