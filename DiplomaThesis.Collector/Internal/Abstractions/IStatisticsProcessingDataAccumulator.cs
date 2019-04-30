using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.DAL.Contracts;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IStatisticsProcessingDataAccumulator
    {
        void PublishTotalDatabaseStatistics(ITotalDatabaseStatistics statistics);
        void PublishViewStatistics(StatementViewStatisticsData statistics);
        void ClearState();
        StatisticsProcessingDataAccumulatorState ProvideState();
    }

    internal class StatisticsProcessingDataAccumulatorState
    {
        public Dictionary<uint, List<TotalRelationStatistics>> TotalRelationStatistics { get; } = new Dictionary<uint, List<TotalRelationStatistics>>();
        public Dictionary<uint, List<TotalIndexStatistics>> TotalIndexStatistics { get; } = new Dictionary<uint, List<TotalIndexStatistics>>();
        public Dictionary<uint, List<TotalStoredProcedureStatistics>> TotalProceduresStatistics { get; } = new Dictionary<uint, List<TotalStoredProcedureStatistics>>();
        public Dictionary<uint, List<TotalViewStatistics>> TotalViewStatistics { get; } = new Dictionary<uint, List<TotalViewStatistics>>();
    }

    internal class StatementViewStatisticsData
    {
        public uint DatabaseID { get; set; }
        public uint ViewID { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}
