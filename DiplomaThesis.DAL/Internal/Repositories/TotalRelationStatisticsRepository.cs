using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalRelationStatisticsRepository : BaseRepository<long, TotalRelationStatistics>, ITotalRelationStatisticsRepository
    {
        public TotalRelationStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.CreatedDate >= dateFrom && x.CreatedDate < dateTo)
                                .GroupBy(x => x.CreatedDate.Date)
                                .Select(x => new { Date = x.Key }).ToList().Count >= Math.Ceiling((dateTo - dateFrom).TotalDays);
            }
        }

        public IEnumerable<TotalRelationStatistics> GetAllForDatabase(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive).ToList();
            }
        }

        public IEnumerable<TotalRelationStatistics> GetAllForRelation(uint relationID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.RelationID == relationID && x.Date >= dateFromInclusive && x.Date < dateToExclusive).ToList();
            }
        }

        public IReadOnlyDictionary<uint, List<TotalRelationStatistics>> GetAllGroupedByRelation(DateTime createdFromInclusive, DateTime createdToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics
                    .Where(x => x.CreatedDate >= createdFromInclusive && x.CreatedDate < createdToExclusive)
                    .GroupBy(x => x.RelationID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public TotalRelationStatistics GetByUniqueKey(TotalRelationStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.RelationID == key.RelationID).SingleOrDefault();
            }
        }

        public IEnumerable<SummaryTotalRelationStatistics> GetSummaryTotalRelationStatistics(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive, int count)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics
                    .Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => x.RelationID)
                    .OrderByDescending(x => x.Sum(y => y.IndexScanCount) + x.Sum(y => y.SeqScanCount) + x.Sum(y => y.TupleDeleteCount)
                                        + x.Sum(y => y.TupleInsertCount) + x.Sum(y => y.TupleUpdateCount))
                    .Take(count)
                    .Select(x => new SummaryTotalRelationStatistics()
                    {
                        DatabaseID = databaseID,
                        IndexScanCount = x.Sum(y => y.IndexScanCount),
                        IndexTupleFetchCount = x.Sum(y => y.IndexTupleFetchCount),
                        RelationID = x.Key,
                        SeqScanCount = x.Sum(y => y.SeqScanCount),
                        SeqTupleReadCount = x.Sum(y => y.SeqTupleReadCount),
                        TupleDeleteCount = x.Sum(y => y.TupleDeleteCount),
                        TupleInsertCount = x.Sum(y => y.TupleInsertCount),
                        TupleUpdateCount = x.Sum(y => y.TupleUpdateCount),
                        TotalLivenessCount = x.Sum(y => y.IndexScanCount) + x.Sum(y => y.SeqScanCount) + x.Sum(y => y.TupleDeleteCount)
                                                + x.Sum(y => y.TupleInsertCount) + x.Sum(y => y.TupleUpdateCount)
                    }).ToList();
            }
        }

        public IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.DatabaseID == databaseID)
                                                   .GroupBy(x => x.RelationID)
                                                   .ToDictionary(x => x.Key, x => x.Max(y => y.Date));
            }
        }
    }
}
