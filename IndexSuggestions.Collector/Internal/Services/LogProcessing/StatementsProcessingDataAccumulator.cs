using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.Collector
{
    internal class StatementsProcessingDataAccumulator : IStatementsProcessingDataAccumulator
    {
        private readonly object lockObject = new object();
        private readonly Dictionary<string, NormalizedStatement> statements = new Dictionary<string, NormalizedStatement>();
        private readonly Dictionary<string, NormalizedStatementStatisticsSampler> statementStatistics = new Dictionary<string, NormalizedStatementStatisticsSampler>();
        private readonly Dictionary<string, NormalizedStatementIndexStatisticsSampler> statementIndexStatistics = new Dictionary<string, NormalizedStatementIndexStatisticsSampler>();
        private readonly Dictionary<string, NormalizedStatementRelationStatisticsSampler> statementRelationStatistics = new Dictionary<string, NormalizedStatementRelationStatisticsSampler>();
        private readonly IDateTimeSelector dateTimeSelector;

        public StatementsProcessingDataAccumulator(IDateTimeSelector dateTimeSelector)
        {
            this.dateTimeSelector = dateTimeSelector;
        }

        public void ClearState()
        {
            lock (lockObject)
            {
                statements.Clear();
                statementStatistics.Clear();
                statementIndexStatistics.Clear();
                statementRelationStatistics.Clear();
            }
        }

        public NormalizedStatement ProvideNormalizedStatement(string fingerprint)
        {
            NormalizedStatement result = null;
            statements.TryGetValue(fingerprint, out result);
            return result;
        }

        public StatementsProcessingDataAccumulatorState ProvideState()
        {
            var result = new StatementsProcessingDataAccumulatorState();
            lock (lockObject)
            {
                foreach (var s in statements)
                {
                    result.Statements.Add(s.Key, s.Value);
                }
                foreach (var s in statementStatistics)
                {
                    var stats = new List<NormalizedStatementStatistics>(s.Value.ProvideSamples());
                    result.StatementStatistics.Add(s.Key, stats);
                }
                foreach (var s in statementIndexStatistics)
                {
                    var stats = new List<NormalizedStatementIndexStatistics>(s.Value.ProvideSamples());
                    result.StatementIndexStatistics.Add(s.Key, stats);
                }
                foreach (var s in statementRelationStatistics)
                {
                    var stats = new List<NormalizedStatementRelationStatistics>(s.Value.ProvideSamples());
                    result.StatementRelationStatistics.Add(s.Key, stats);
                }
            }
            return result;
        }

        public void PublishNormalizedStatement(LogEntryStatementData statementData)
        {
            lock (lockObject)
            {
                statements.TryAdd(statementData.NormalizedStatementFingerprint, new NormalizedStatement()
                {
                    Statement = statementData.NormalizedStatement,
                    StatementFingerprint = statementData.NormalizedStatementFingerprint
                }); 
            }
        }

        public void PublishNormalizedStatementDefinition(string fingerprint, StatementDefinition definition)
        {
            lock (lockObject)
            {
                statements[fingerprint].CommandType = definition.CommandType;
                statements[fingerprint].StatementDefinition = definition; 
            }
        }

        public void PublishNormalizedStatementIndexStatistics(LogEntryStatementIndexStatisticsData indexStatisticsData)
        {
            var key = indexStatisticsData.NormalizedStatementFingerprint;
            var sample = new NormalizedStatementIndexStatistics()
            {
                AvgTotalCost = indexStatisticsData.TotalCost,
                CreatedDate = DateTime.Now,
                DatabaseID = indexStatisticsData.DatabaseID,
                Date = indexStatisticsData.ExecutionDate,
                IndexID = indexStatisticsData.IndexID,
                MaxTotalCost = indexStatisticsData.TotalCost,
                MinTotalCost = indexStatisticsData.TotalCost,
                TotalIndexScanCount = 1
            };
            lock (lockObject)
            {
                if (!statementIndexStatistics.ContainsKey(key))
                {
                    statementIndexStatistics.Add(key, new NormalizedStatementIndexStatisticsSampler(dateTimeSelector, null));
                }
            }
            lock (String.Intern(key))
            {
                statementIndexStatistics[key].AddSample(sample);
            }
        }

        public void PublishNormalizedStatementRelationStatistics(LogEntryStatementRelationStatisticsData relationStatisticsData)
        {
            var key = relationStatisticsData.NormalizedStatementFingerprint;
            var sample = new NormalizedStatementRelationStatistics()
            {
                AvgTotalCost = relationStatisticsData.TotalCost,
                CreatedDate = DateTime.Now,
                DatabaseID = relationStatisticsData.DatabaseID,
                Date = relationStatisticsData.ExecutionDate,
                RelationID = relationStatisticsData.RelationID,
                MaxTotalCost = relationStatisticsData.TotalCost,
                MinTotalCost = relationStatisticsData.TotalCost,
                TotalScansCount = 1
            };
            lock (lockObject)
            {
                if (!statementRelationStatistics.ContainsKey(key))
                {
                    statementRelationStatistics.Add(key, new NormalizedStatementRelationStatisticsSampler(dateTimeSelector, null));
                }
            }
            lock (String.Intern(key))
            {
                statementRelationStatistics[key].AddSample(sample);
            }
        }

        public void PublishNormalizedStatementStatistics(LogEntryStatementStatisticsData statementStatisticsData)
        {
            var key = statementStatisticsData.NormalizedStatementFingerprint;
            var sample = new NormalizedStatementStatistics()
            {
                ApplicationName = statementStatisticsData.ApplicationName,
                AvgDuration = statementStatisticsData.Duration,
                CreatedDate = DateTime.Now,
                DatabaseID = statementStatisticsData.DatabaseID,
                Date = statementStatisticsData.ExecutionDate,
                MaxDuration = statementStatisticsData.Duration,
                MinDuration = statementStatisticsData.Duration,
                RepresentativeStatement = statementStatisticsData.Statement,
                TotalExecutionsCount = 1,
                UserName = statementStatisticsData.UserName
            };
            lock (lockObject)
            {
                if (!statementStatistics.ContainsKey(key))
                {
                    statementStatistics.Add(key, new NormalizedStatementStatisticsSampler(dateTimeSelector, null));
                }
            }
            lock (String.Intern(key))
            {
                statementStatistics[key].AddSample(sample);
            }
        }
    }
}
