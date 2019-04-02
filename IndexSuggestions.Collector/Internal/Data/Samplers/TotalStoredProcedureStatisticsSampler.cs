using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class TotalStoredProcedureStatisticsSampler : DataSampler<TotalStoredProcedureStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public TotalStoredProcedureStatisticsSampler(IDateTimeSelector dateTimeSelector, TotalStoredProcedureStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData);
            }
        }
        protected override void ApplySampling(TotalStoredProcedureStatistics cumulativeData, TotalStoredProcedureStatistics newSample)
        {
            TotalStoredProcedureStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(TotalStoredProcedureStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.ProcedureID}_{dateKey}";
        }

        protected override void InitializeCumulativeData(TotalStoredProcedureStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
