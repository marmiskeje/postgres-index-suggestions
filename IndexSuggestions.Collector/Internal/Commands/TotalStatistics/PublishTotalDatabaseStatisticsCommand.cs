using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class PublishTotalDatabaseStatisticsCommand : ChainableCommand
    {
        private readonly TotalStatisticsCollectNextSampleContext context;
        private readonly IStatisticsProcessingDataAccumulator accumulator;

        public PublishTotalDatabaseStatisticsCommand(TotalStatisticsCollectNextSampleContext context, IStatisticsProcessingDataAccumulator accumulator)
        {
            this.context = context;
            this.accumulator = accumulator;
        }
        protected override void OnExecute()
        {
            foreach (var s in context.TotalDatabaseStatistics)
            {
                accumulator.PublishTotalDatabaseStatistics(s.Value);
            }
        }
    }
}
