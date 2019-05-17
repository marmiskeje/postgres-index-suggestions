using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalStoredProcedureStatisticsRepository : BaseRepository<long, TotalStoredProcedureStatistics>, ITotalStoredProcedureStatisticsRepository
    {
        public TotalStoredProcedureStatisticsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate < dateTo)
                                .GroupBy(x => x.CreatedDate.Date)
                                .Select(x => new { Date = x.Key }).ToList().Count >= Math.Ceiling((dateTo - dateFrom).TotalDays);
            }
        }

        public IEnumerable<TotalStoredProcedureStatistics> GetAllForProcedure(uint storedProcedureID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics.Where(x => x.ProcedureID == storedProcedureID && x.Date >= dateFromInclusive && x.Date < dateToExclusive).ToList();
            }
        }

        public IReadOnlyDictionary<uint, List<TotalStoredProcedureStatistics>> GetAllGroupedByProcedure(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.ProcedureID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public TotalStoredProcedureStatistics GetByUniqueKey(TotalStoredProcedureStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics.Where(x => x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.ProcedureID == key.ProcedureID).SingleOrDefault();
            }
        }

        public IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics.Where(x => x.DatabaseID == databaseID)
                                                   .GroupBy(x => x.ProcedureID)
                                                   .ToDictionary(x => x.Key, x => x.Max(y => y.Date));
            }
        }
    }
}
