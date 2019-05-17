using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class NormalizedStatementRelationStatisticsRepository : BaseRepository<long, NormalizedStatementRelationStatistics>, INormalizedStatementRelationStatisticsRepository
    {
        public NormalizedStatementRelationStatisticsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IEnumerable<NormalizedStatementRelationStatistics> GetAllForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementRelationStatistics
                    .Where(x => x.NormalizedStatementID == normalizedStatementID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .ToList();
            }
        }

        public IReadOnlyDictionary<long, List<NormalizedStatementRelationStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementRelationStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.NormalizedStatementID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public NormalizedStatementRelationStatistics GetByUniqueKey(NormalizedStatementRelationStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementRelationStatistics.Where(x => x.DatabaseID == key.DatabaseID
                                && x.Date == key.Date && x.RelationID == key.RelationID
                                && x.NormalizedStatementID == key.NormalizedStatementID).SingleOrDefault();
            }
        }

        public IEnumerable<RelationSummaryStatementStatistics> GetRelationSummaryStatementStatistics(uint relationID, DateTime dateFromInclusive, DateTime dateToExclusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementRelationStatistics
                    .Where(x => x.RelationID == relationID && x.Date >= dateFromInclusive && x.Date < dateToExclusive)
                    .GroupBy(x => new { x.NormalizedStatement.ID, x.NormalizedStatement.Statement })
                    .OrderByDescending(x => x.Max(y => y.MaxTotalCost))
                    .Select(x => new RelationSummaryStatementStatistics()
                    {
                        AvgCost = x.Average(y => y.AvgTotalCost),
                        MaxCost = x.Max(y => y.MaxTotalCost),
                        MinCost = x.Min(y => y.MinTotalCost),
                        SeqScansCount = x.Sum(y => y.TotalScansCount),
                        NormalizedStatement = x.Key.Statement,
                        NormalizedStatementID = x.Key.ID
                    }).ToList();
            }
        }
    }
}
