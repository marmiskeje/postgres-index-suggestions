using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class NormalizedStatementStatisticsRepository : BaseRepository<long, NormalizedStatementStatistics>, INormalizedStatementStatisticsRepository
    {
        public NormalizedStatementStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IEnumerable<NormalizedStatementStatisticsTimeline> GetTimelineForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics.Where(x => x.NormalizedStatementID == normalizedStatementID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => new { x.Date })
                    .Select(x => new NormalizedStatementStatisticsTimeline()
                    {
                        AvgDuration = TimeSpan.FromTicks((long)x.Average(y => y.AvgDuration.Ticks)),
                        AvgTotalCost = x.Average(y => y.AvgTotalCost),
                        Date = x.Key.Date,
                        MaxDuration = x.Max(y => y.MaxDuration),
                        MaxTotalCost = x.Max(y => y.MaxTotalCost),
                        MinDuration = x.Min(y => y.MinDuration),
                        MinTotalCost = x.Min(y => y.MinTotalCost),
                        TotalDuration = TimeSpan.FromTicks(x.Sum(y => y.TotalDuration.Ticks)),
                        TotalExecutionsCount = x.Sum(y => y.TotalExecutionsCount)
                    }).ToList();
            }
        }

        public IReadOnlyDictionary<long, List<NormalizedStatementStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.NormalizedStatementID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics.Where(x => x.ApplicationName == key.ApplicationName
                        && x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.NormalizedStatementID == key.NormalizedStatementID
                        && x.UserName == key.UserName).SingleOrDefault();
            }
        }

        public IEnumerable<NormalizedStatementStatistics> GetSlowestForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive, int count)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics
                    .Where(x => x.NormalizedStatementID == normalizedStatementID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .OrderByDescending(x => x.MaxDuration)
                    .Take(count).ToList();
            }
        }

        public IEnumerable<SummaryNormalizedStatementStatistics> GetSummaryTotalStatementStatistics(uint databaseID,
                        DateTime dateFromInclusive, DateTime dateToExclusive, SummaryNormalizedStatementStatisticsOrderBy orderBy,
                        StatementQueryCommandType? commandType, int? count)
        {
            using (var context = CreateContextFunc())
            {
                var query = context.NormalizedStatementStatistics
                    .Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive);
                if (commandType.HasValue)
                {
                    query = query.Where(x => x.NormalizedStatement.CommandType == commandType.Value);
                }
                var aggregateQuery = query.GroupBy(x => new { x.NormalizedStatement.ID, x.NormalizedStatement.Statement, x.NormalizedStatement.CommandType });
                switch (orderBy)
                {
                    case SummaryNormalizedStatementStatisticsOrderBy.ExecutionCount:
                        aggregateQuery = aggregateQuery.OrderByDescending(x => x.Sum(y => y.TotalExecutionsCount));
                        break;
                    case SummaryNormalizedStatementStatisticsOrderBy.MaxDuration:
                        aggregateQuery = aggregateQuery.OrderByDescending(x => x.Max(y => y.MaxDuration));
                        break;
                }
                if (count.HasValue)
                {
                    aggregateQuery = aggregateQuery.Take(count.Value);
                }
                return aggregateQuery
                             .Select(x => new SummaryNormalizedStatementStatistics()
                             {
                                 AvgDuration = TimeSpan.FromTicks((long)x.Average(y => y.AvgDuration.Ticks)),
                                 AvgTotalCost = x.Average(y => y.AvgTotalCost),
                                 CommandType = x.Key.CommandType,
                                 MaxDuration = x.Max(y => y.MaxDuration),
                                 MaxTotalCost = x.Max(y => y.MaxTotalCost),
                                 MinDuration = x.Min(y => y.MinDuration),
                                 MinTotalCost = x.Min(y => y.MinTotalCost),
                                 Statement = x.Key.Statement,
                                 StatementID = x.Key.ID,
                                 TotalDuration = TimeSpan.FromTicks(x.Sum(y => y.TotalDuration.Ticks)),
                                 TotalExecutionsCount = x.Sum(y => y.TotalExecutionsCount)
                             }).ToList();
            }
        }
    }
}
