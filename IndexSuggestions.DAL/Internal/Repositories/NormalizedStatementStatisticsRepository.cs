using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
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

        public IReadOnlyDictionary<long, NormalizedStatementStatistics> GetTotalGroupedByStatement(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics
                    .Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => x.NormalizedStatementID)
                    .ToDictionary(x => x.Key, x => new NormalizedStatementStatistics()
                    {
                        DatabaseID = databaseID,
                        Date = dateFromInclusive,
                        CreatedDate = x.Max(y => y.CreatedDate),
                        AvgDuration = TimeSpan.FromTicks((long)x.Average(y => y.AvgDuration.Ticks)),
                        MaxDuration = x.Max(y => y.MaxDuration),
                        MinDuration = x.Min(y => y.MinDuration),
                        NormalizedStatementID = x.Key,
                        TotalDuration = TimeSpan.FromTicks(x.Sum(y => y.TotalDuration.Ticks)),
                        TotalExecutionsCount = x.Sum(y => y.TotalExecutionsCount)
                    });
            }
        }
    }
}
