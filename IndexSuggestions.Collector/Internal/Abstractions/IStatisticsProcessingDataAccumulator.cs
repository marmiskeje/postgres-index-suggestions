using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IStatisticsProcessingDataAccumulator
    {
        void PublishTotalDatabaseStatistics(ITotalDatabaseStatistics statistics);
        void ClearState();
        StatisticsProcessingDataAccumulatorState ProvideState();
    }

    internal class StatisticsProcessingDataAccumulatorState
    {
        public Dictionary<uint, List<TotalRelationStatistics>> TotalRelationStatistics { get; } = new Dictionary<uint, List<TotalRelationStatistics>>();
        public Dictionary<uint, List<TotalIndexStatistics>> TotalIndexStatistics { get; } = new Dictionary<uint, List<TotalIndexStatistics>>();
        public Dictionary<uint, List<TotalStoredProcedureStatistics>> TotalProceduresStatistics { get; } = new Dictionary<uint, List<TotalStoredProcedureStatistics>>();
    }
}
