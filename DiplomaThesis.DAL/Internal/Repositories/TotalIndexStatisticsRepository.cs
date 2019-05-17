using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalIndexStatisticsRepository : BaseRepository<long, TotalIndexStatistics>, ITotalIndexStatisticsRepository
    {
        public TotalIndexStatisticsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalIndexStatistics.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate < dateTo)
                                .GroupBy(x => x.CreatedDate.Date)
                                .Select(x => new { Date = x.Key }).ToList().Count >= Math.Ceiling((dateTo - dateFrom).TotalDays);
            }
        }

        public IEnumerable<TotalIndexStatistics> GetAllForIndex(uint indexID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalIndexStatistics.Where(x => x.IndexID == indexID && x.Date >= dateFromInclusive && x.Date < dateToExclusive).ToList();
            }
        }

        public IReadOnlyDictionary<uint, List<TotalIndexStatistics>> GetAllGroupedByIndex(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalIndexStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.IndexID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public TotalIndexStatistics GetByUniqueKey(TotalIndexStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalIndexStatistics.Where(x => x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.RelationID == key.RelationID
                                        && x.IndexID == key.IndexID).SingleOrDefault();
            }
        }

        public IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalIndexStatistics.Where(x => x.DatabaseID == databaseID)
                                                   .GroupBy(x => x.IndexID)
                                                   .ToDictionary(x => x.Key, x => x.Max(y => y.Date));
            }
        }
    }
}
