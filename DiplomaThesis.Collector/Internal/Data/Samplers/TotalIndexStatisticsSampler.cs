using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class TotalIndexStatisticsSampler : DataSampler<TotalIndexStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public TotalIndexStatisticsSampler(IDateTimeSelector dateTimeSelector, TotalIndexStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData);
            }
        }
        protected override void ApplySampling(TotalIndexStatistics cumulativeData, TotalIndexStatistics newSample)
        {
            TotalIndexStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(TotalIndexStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.RelationID}_{data.IndexID}_{dateKey}";
        }

        protected override void InitializeCumulativeData(TotalIndexStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
