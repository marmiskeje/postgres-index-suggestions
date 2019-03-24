using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IStatementDataAccumulator
    {
        NormalizedStatement ProvideNormalizedStatement(string fingerprint);
        void PublishNormalizedStatement(LogEntryStatementData statementData);
        void PublishNormalizedStatementStatistics(LogEntryStatementStatisticsData statementStatisticsData);
        void PublishNormalizedStatementDefinition(string fingerprint, StatementDefinition statementDefinition);
        void PublishNormalizedStatementIndexStatistics(LogEntryStatementIndexStatisticsData indexStatisticsData);

        StatementDataAccumulatorState ProvideState();
        void ClearState();
    }

    internal class StatementDataAccumulatorState
    {
        public Dictionary<string, NormalizedStatement> Statements { get; } = new Dictionary<string, NormalizedStatement>();
        public Dictionary<string, List<NormalizedStatementStatistics>> StatementStatistics { get; } = new Dictionary<string, List<NormalizedStatementStatistics>>();
        public Dictionary<string, List<NormalizedStatementIndexStatistics>> StatementIndexStatistics { get; } = new Dictionary<string, List<NormalizedStatementIndexStatistics>>();
    }
}
