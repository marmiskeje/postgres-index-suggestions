using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.Cache;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.Collector
{
    internal class StatisticsProcessingDataAccumulator : IStatisticsProcessingDataAccumulator
    {
        private readonly static TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(5);
        private const string DATABASE_LAST_STATS_RESET_CACHEKEY_FORMAT = "StatisticsProcessingDataAccumulator_DatabaseLastStatsReset_{0}";
        private const string LAST_VALUE_CACHEKEY_FORMAT = "StatisticsProcessingDataAccumulator_LastValue_Entity_{0}_ID_{1}";
        private readonly IDateTimeSelector dateTimeSelector;
        private readonly ICache cache;
        private readonly ConcurrentDictionary<uint, TotalRelationStatisticsSampler> relationStatistics = new ConcurrentDictionary<uint, TotalRelationStatisticsSampler>();
        private readonly ConcurrentDictionary<uint, TotalIndexStatisticsSampler> indexStatistics = new ConcurrentDictionary<uint, TotalIndexStatisticsSampler>();
        private readonly ConcurrentDictionary<uint, TotalStoredProcedureStatisticsSampler> procedureStatistics = new ConcurrentDictionary<uint, TotalStoredProcedureStatisticsSampler>();

        public StatisticsProcessingDataAccumulator(IDateTimeSelector dateTimeSelector, ICache cache)
        {
            this.dateTimeSelector = dateTimeSelector;
            this.cache = cache;
        }
        public void PublishTotalDatabaseStatistics(ITotalDatabaseStatistics statistics)
        {
            DateTime lastStatReset = DateTime.MinValue;
            DateTime newStatReset = statistics.Database.LastResetDate ?? DateTime.MinValue;
            uint databaseId = statistics.Database.ID;
            var databaseLastStatsResetCacheKey = String.Format(DATABASE_LAST_STATS_RESET_CACHEKEY_FORMAT, databaseId);
            if (!cache.TryGetValue<DateTime>(databaseLastStatsResetCacheKey, out lastStatReset))
            {
                lastStatReset = DateTime.MinValue;
            }
            cache.Save(databaseLastStatsResetCacheKey, newStatReset, CACHE_EXPIRATION);
            bool isStatisticsResetDetected = (newStatReset - lastStatReset).TotalSeconds != 0;
            ProcessRelations(statistics.Relations, statistics.CollectedDate, isStatisticsResetDetected);
            ProcessIndices(statistics.Indices, statistics.CollectedDate, isStatisticsResetDetected);
            ProcessProcedures(statistics.StoredProcedures, statistics.CollectedDate, isStatisticsResetDetected);
        }

        private TData GetLastValue<TData>(long id)
        {
            var key = string.Format(LAST_VALUE_CACHEKEY_FORMAT, typeof(TData).Name, id);
            TData result = default(TData);
            cache.TryGetValue(key, out result);
            return result;
        }
        private void SaveLastValue<TData>(long id, TData data)
        {
            var key = string.Format(LAST_VALUE_CACHEKEY_FORMAT, typeof(TData).Name, id);
            cache.Save(key, data, CACHE_EXPIRATION);
        }

        private void ProcessRelations(IEnumerable<IRelationStatistics> relations, DateTime collectedDate, bool isStatisticsResetDetected)
        {
            foreach (var r in relations)
            {
                var lastValue = GetLastValue<IRelationStatistics>(r.ID);
                TotalRelationStatistics diff = null;
                if (!isStatisticsResetDetected && lastValue != null && !r.Equals(lastValue))
                {
                    diff = new TotalRelationStatistics()
                    {
                        CreatedDate = DateTime.Now,
                        DatabaseID = r.DatabaseID,
                        Date = collectedDate,
                        IndexScanCount = Math.Max(r.IndexScanCount - lastValue.IndexScanCount, 0),
                        IndexTupleFetchCount = Math.Max(r.IndexTupleFetchCount - lastValue.IndexTupleFetchCount, 0),
                        RelationID = r.ID,
                        SeqScanCount = Math.Max(r.SeqScanCount - lastValue.SeqScanCount, 0),
                        SeqTupleReadCount = Math.Max(r.SeqTupleReadCount - lastValue.SeqTupleReadCount, 0),
                        TupleDeleteCount = Math.Max(r.TupleDeleteCount - lastValue.TupleDeleteCount, 0),
                        TupleInsertCount = Math.Max(r.TupleInsertCount - lastValue.TupleInsertCount, 0),
                        TupleUpdateCount = Math.Max(r.TupleUpdateCount - lastValue.TupleUpdateCount, 0)
                    };
                }
                relationStatistics.AddOrUpdate(r.ID, new TotalRelationStatisticsSampler(dateTimeSelector, diff), (k, v) =>
                {
                    if (diff != null)
                    {
                        v.AddSample(diff);
                    }
                    return v;
                });
                SaveLastValue(r.ID, r);
            }
        }

        private void ProcessIndices(IEnumerable<IIndexStatistics> indices, DateTime collectedDate, bool isStatisticsResetDetected)
        {
            foreach (var i in indices)
            {
                var lastValue = GetLastValue<IIndexStatistics>(i.ID);
                TotalIndexStatistics diff = null;
                if (!isStatisticsResetDetected && lastValue != null && !i.Equals(lastValue))
                {
                    diff = new TotalIndexStatistics()
                    {
                        CreatedDate = DateTime.Now,
                        DatabaseID = i.DatabaseID,
                        Date = collectedDate,
                        IndexID = i.ID,
                        IndexScanCount = Math.Max(i.IndexScanCount - lastValue.IndexScanCount, 0),
                        IndexTupleFetchCount = Math.Max(i.IndexTupleFetchCount - lastValue.IndexTupleFetchCount, 0),
                        IndexTupleReadCount = Math.Max(i.IndexTupleReadCount - lastValue.IndexTupleReadCount, 0),
                        RelationID = i.RelationID
                    };
                }
                indexStatistics.AddOrUpdate(i.ID, new TotalIndexStatisticsSampler(dateTimeSelector, diff), (k, v) =>
                {
                    if (diff != null)
                    {
                        v.AddSample(diff);
                    }
                    return v;
                });
                SaveLastValue(i.ID, i);
            }
        }

        private void ProcessProcedures(IEnumerable<IStoredProcedureStatistics> procedures, DateTime collectedDate, bool isStatisticsResetDetected)
        {
            foreach (var p in procedures)
            {
                var lastValue = GetLastValue<IStoredProcedureStatistics>(p.ID);
                TotalStoredProcedureStatistics diff = null;
                if (!isStatisticsResetDetected && lastValue != null && !p.Equals(lastValue))
                {
                    diff = new TotalStoredProcedureStatistics()
                    {
                        CreatedDate = DateTime.Now,
                        CallsCount = Math.Max(p.CallsCount - lastValue.CallsCount, 0),
                        DatabaseID = p.DatabaseID,
                        Date = collectedDate,
                        ProcedureID = p.ID,
                        SelfDurationInMs = Math.Max(p.SelfDurationInMs - lastValue.SelfDurationInMs, 0),
                        TotalDurationInMs = Math.Max(p.TotalDurationInMs - lastValue.TotalDurationInMs, 0)
                    };
                }
                procedureStatistics.AddOrUpdate(p.ID, new TotalStoredProcedureStatisticsSampler(dateTimeSelector, diff), (k, v) =>
                {
                    if (diff != null)
                    {
                        v.AddSample(diff);
                    }
                    return v;
                });
                SaveLastValue(p.ID, p);
            }
        }

        public void ClearState()
        {
            relationStatistics.Clear();
            indexStatistics.Clear();
            procedureStatistics.Clear();
        }

        public StatisticsProcessingDataAccumulatorState ProvideState()
        {
            StatisticsProcessingDataAccumulatorState result = new StatisticsProcessingDataAccumulatorState();
            foreach (var r in relationStatistics)
            {
                var stats = new List<TotalRelationStatistics>(r.Value.ProvideSamples());
                result.TotalRelationStatistics.Add(r.Key, stats);
            }
            foreach (var i in indexStatistics)
            {
                var stats = new List<TotalIndexStatistics>(i.Value.ProvideSamples());
                result.TotalIndexStatistics.Add(i.Key, stats);
            }
            foreach (var p in procedureStatistics)
            {
                var stats = new List<TotalStoredProcedureStatistics>(p.Value.ProvideSamples());
                result.TotalProceduresStatistics.Add(p.Key, stats);
            }
            return result;
        }
    }
}
