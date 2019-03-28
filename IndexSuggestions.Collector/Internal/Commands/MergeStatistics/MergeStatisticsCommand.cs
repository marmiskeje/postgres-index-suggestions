using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class MergeStatisticsCommand<TKey, TData> : ChainableCommand
    {
        private readonly MergeStatisticsContext<TKey, TData> context;
        private readonly Func<DataSampler<TData>> createSamplerFunc;
        public MergeStatisticsCommand(MergeStatisticsContext<TKey, TData> context, Func<DataSampler<TData>> createSamplerFunc)
        {
            this.context = context;
            this.createSamplerFunc = createSamplerFunc;
        }
        protected override void OnExecute()
        {
            foreach (var s in context.LoadedStatistics)
            {
                var sampler = createSamplerFunc();
                foreach (var item in s.Value)
                {
                    sampler.AddSample(item);
                }
                context.MergedStatistics.Add(s.Key, sampler.ProvideSamples());
            }
        }
    }
}
