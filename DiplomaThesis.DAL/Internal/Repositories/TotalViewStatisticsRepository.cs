using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalViewStatisticsRepository : BaseRepository<long, TotalViewStatistics>, ITotalViewStatisticsRepository
    {
        public TotalViewStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalViewStatistics.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate < dateTo)
                                .GroupBy(x => x.CreatedDate.Date)
                                .Select(x => new { Date = x.Key }).ToList().Count >= Math.Ceiling((dateTo - dateFrom).TotalDays);
            }
        }

        public IReadOnlyDictionary<uint, List<TotalViewStatistics>> GetAllGroupedByView(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalViewStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.ViewID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public TotalViewStatistics GetByUniqueKey(TotalViewStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalViewStatistics.Where(x => x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.ViewID == key.ViewID).SingleOrDefault();
            }
        }

        public IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalViewStatistics.Where(x => x.DatabaseID == databaseID)
                                                   .GroupBy(x => x.ViewID)
                                                   .ToDictionary(x => x.Key, x => x.Max(y => y.Date));
            }
        }
    }
}
