using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class TotalRelationStatisticsSampler : DataSampler<TotalRelationStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public TotalRelationStatisticsSampler(IDateTimeSelector dateTimeSelector, TotalRelationStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData);
            }
        }
        protected override void ApplySampling(TotalRelationStatistics cumulativeData, TotalRelationStatistics newSample)
        {
            TotalRelationStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(TotalRelationStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.RelationID}_{dateKey}";
        }

        protected override void InitializeCumulativeData(TotalRelationStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
