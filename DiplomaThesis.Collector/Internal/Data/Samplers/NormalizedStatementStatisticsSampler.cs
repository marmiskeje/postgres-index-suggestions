using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class NormalizedStatementStatisticsSampler : DataSampler<NormalizedStatementStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public NormalizedStatementStatisticsSampler(IDateTimeSelector dateTimeSelector, NormalizedStatementStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData); 
            }
        }
        protected override void ApplySampling(NormalizedStatementStatistics cumulativeData, NormalizedStatementStatistics newSample)
        {
            NormalizedStatementStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(NormalizedStatementStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.UserName}_{data.ApplicationName}_{dateKey}";
        }

        protected override void InitializeCumulativeData(NormalizedStatementStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
