using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class TotalViewStatisticsSampler : DataSampler<TotalViewStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public TotalViewStatisticsSampler(IDateTimeSelector dateTimeSelector, TotalViewStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData);
            }
        }
        protected override void ApplySampling(TotalViewStatistics cumulativeData, TotalViewStatistics newSample)
        {
            TotalViewStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(TotalViewStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.ViewID}_{dateKey}";
        }

        protected override void InitializeCumulativeData(TotalViewStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
