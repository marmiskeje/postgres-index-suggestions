using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class LoadStatisticsForMergeCommand<TKey, TData> : ChainableCommand
    {
        private readonly MergeStatisticsContext<TKey, TData> context;
        private readonly Func<DateTime, DateTime, IReadOnlyDictionary<TKey, List<TData>>> loadingFunc;
        public LoadStatisticsForMergeCommand(MergeStatisticsContext<TKey, TData> context, Func<DateTime, DateTime, IReadOnlyDictionary<TKey, List<TData>>> loadingFunc)
        {
            this.context = context;
            this.loadingFunc = loadingFunc;
        }
        protected override void OnExecute()
        {
            context.LoadedStatistics = loadingFunc(context.CreatedDateFrom, context.CreatedDateTo);
        }
    }
}
