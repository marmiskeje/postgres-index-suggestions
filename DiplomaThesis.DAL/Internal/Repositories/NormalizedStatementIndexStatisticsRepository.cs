using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class NormalizedStatementIndexStatisticsRepository : BaseRepository<long, NormalizedStatementIndexStatistics>, INormalizedStatementIndexStatisticsRepository
    {
        public NormalizedStatementIndexStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IEnumerable<NormalizedStatementIndexStatistics> GetAllForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementIndexStatistics
                    .Where(x => x.NormalizedStatementID == normalizedStatementID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .ToList();
            }
        }

        public IReadOnlyDictionary<long, List<NormalizedStatementIndexStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementIndexStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.NormalizedStatementID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public NormalizedStatementIndexStatistics GetByUniqueKey(NormalizedStatementIndexStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementIndexStatistics.Where(x => x.DatabaseID == key.DatabaseID
                                && x.Date == key.Date && x.IndexID == key.IndexID
                                && x.NormalizedStatementID == key.NormalizedStatementID).SingleOrDefault();
            }
        }

        public IEnumerable<IndexSummaryStatementStatistics> GetIndexSummaryStatementStatistics(uint indexID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementIndexStatistics
                    .Where(x => x.IndexID == indexID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => new { x.NormalizedStatement.ID, x.NormalizedStatement.Statement })
                    .OrderByDescending(x => x.Max(y => y.MaxTotalCost))
                    .Select(x => new IndexSummaryStatementStatistics()
                    {
                        AvgCost = x.Average(y => y.AvgTotalCost),
                        MaxCost = x.Max(y => y.MaxTotalCost),
                        MinCost = x.Min(y => y.MinTotalCost),
                        IndexScansCount = x.Sum(y => y.TotalIndexScanCount),
                        NormalizedStatement = x.Key.Statement,
                        NormalizedStatementID = x.Key.ID
                    }).ToList();
            }
        }
    }
}
