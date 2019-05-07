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

        public IEnumerable<SummaryNormalizedStatementStatistics> GetSummaryTotalStatementStatistics(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive, SummaryNormalizedStatementStatisticsOrderBy orderBy, int count)
        {
            using (var context = CreateContextFunc())
            {
                var query = context.NormalizedStatementStatistics
                    .Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => x.NormalizedStatement.Statement);
                switch (orderBy)
                {
                    case SummaryNormalizedStatementStatisticsOrderBy.ExecutionCount:
                        query = query.OrderByDescending(x => x.Sum(y => y.TotalExecutionsCount));
                        break;
                    case SummaryNormalizedStatementStatisticsOrderBy.MaxDuration:
                        query = query.OrderByDescending(x => x.Max(y => y.MaxDuration));
                        break;
                }
                return query.Take(count)
                             .Select(x => new SummaryNormalizedStatementStatistics()
                             {
                                 AvgDuration = TimeSpan.FromTicks((long)x.Average(y => y.AvgDuration.Ticks)),
                                 MaxDuration = x.Max(y => y.MaxDuration),
                                 MinDuration = x.Min(y => y.MinDuration),
                                 Statement = x.Key,
                                 TotalDuration = TimeSpan.FromTicks(x.Sum(y => y.TotalDuration.Ticks)),
                                 TotalExecutionsCount = x.Sum(y => y.TotalExecutionsCount)
                             }).ToList();
            }
        }
    }
}
