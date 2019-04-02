using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IStatementsProcessingDataAccumulator
    {
        NormalizedStatement ProvideNormalizedStatement(string fingerprint);
        void PublishNormalizedStatement(LogEntryStatementData statementData);
        void PublishNormalizedStatementStatistics(LogEntryStatementStatisticsData statementStatisticsData);
        void PublishNormalizedStatementDefinition(string fingerprint, StatementDefinition statementDefinition);
        void PublishNormalizedStatementIndexStatistics(LogEntryStatementIndexStatisticsData indexStatisticsData);
        void PublishNormalizedStatementRelationStatistics(LogEntryStatementRelationStatisticsData relationStatisticsData);

        StatementsProcessingDataAccumulatorState ProvideState();
        void ClearState();
    }

    internal class StatementsProcessingDataAccumulatorState
    {
        public Dictionary<string, NormalizedStatement> Statements { get; } = new Dictionary<string, NormalizedStatement>();
        public Dictionary<string, List<NormalizedStatementStatistics>> StatementStatistics { get; } = new Dictionary<string, List<NormalizedStatementStatistics>>();
        public Dictionary<string, List<NormalizedStatementIndexStatistics>> StatementIndexStatistics { get; } = new Dictionary<string, List<NormalizedStatementIndexStatistics>>();
        public Dictionary<string, List<NormalizedStatementRelationStatistics>> StatementRelationStatistics { get; } = new Dictionary<string, List<NormalizedStatementRelationStatistics>>();
    }
}
