using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class NormalizedStatementIndexStatisticsSampler : DataSampler<NormalizedStatementIndexStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public NormalizedStatementIndexStatisticsSampler(IDateTimeSelector dateTimeSelector, NormalizedStatementIndexStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData); 
            }
        }
        protected override void ApplySampling(NormalizedStatementIndexStatistics cumulativeData, NormalizedStatementIndexStatistics newSample)
        {
            NormalizedStatementIndexStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(NormalizedStatementIndexStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.IndexID}_{dateKey}";
        }
        protected override void InitializeCumulativeData(NormalizedStatementIndexStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
