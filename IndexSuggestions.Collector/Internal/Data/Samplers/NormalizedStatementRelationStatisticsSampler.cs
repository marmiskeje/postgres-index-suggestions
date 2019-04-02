using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class NormalizedStatementRelationStatisticsSampler : DataSampler<NormalizedStatementRelationStatistics>
    {
        private readonly IDateTimeSelector dateTimeSelector;
        public NormalizedStatementRelationStatisticsSampler(IDateTimeSelector dateTimeSelector, NormalizedStatementRelationStatistics cumulativeData)
        {
            this.dateTimeSelector = dateTimeSelector;
            if (cumulativeData != null)
            {
                AddSample(cumulativeData); 
            }
        }
        protected override void ApplySampling(NormalizedStatementRelationStatistics cumulativeData, NormalizedStatementRelationStatistics newSample)
        {
            NormalizedStatementRelationStatisticsMergeUtility.ApplySample(cumulativeData, newSample);
        }

        protected override string CreateKey(NormalizedStatementRelationStatistics data)
        {
            string dateKey = String.Format("{0:u}", dateTimeSelector.Select(data.Date));
            return $"{data.DatabaseID}_{data.RelationID}_{dateKey}";
        }

        protected override void InitializeCumulativeData(NormalizedStatementRelationStatistics cumulativeData)
        {
            cumulativeData.Date = dateTimeSelector.Select(cumulativeData.Date);
        }
    }
}
