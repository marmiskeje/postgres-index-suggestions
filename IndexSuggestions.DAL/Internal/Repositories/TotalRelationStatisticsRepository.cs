using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class TotalRelationStatisticsRepository : BaseRepository<long, TotalRelationStatistics>, ITotalRelationStatisticsRepository
    {
        public TotalRelationStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IEnumerable<TotalRelationStatistics> GetAllForDatabase(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics.Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive).ToList();
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

        public IReadOnlyDictionary<uint, TotalRelationStatistics> GetTotalGroupedByRelation(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics
                    .Where(x => x.DatabaseID == databaseID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => x.RelationID)
                    .ToDictionary(x => x.Key, x => new TotalRelationStatistics()
                    {
                        DatabaseID = databaseID,
                        Date = dateFromInclusive,
                        CreatedDate = x.Max(y => y.CreatedDate),
                        IndexScanCount = x.Sum(y => y.IndexScanCount),
                        IndexTupleFetchCount = x.Sum(y => y.IndexTupleFetchCount),
                        RelationID = x.Key,
                        SeqScanCount = x.Sum(y => y.SeqScanCount),
                        SeqTupleReadCount = x.Sum(y => y.SeqTupleReadCount),
                        TupleDeleteCount = x.Sum(y => y.TupleDeleteCount),
                        TupleInsertCount = x.Sum(y => y.TupleInsertCount),
                        TupleUpdateCount = x.Sum(y => y.TupleUpdateCount)
                    });
            }
        }
    }
}
