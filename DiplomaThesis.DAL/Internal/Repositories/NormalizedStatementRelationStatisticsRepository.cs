using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class NormalizedStatementRelationStatisticsRepository : BaseRepository<long, NormalizedStatementRelationStatistics>, INormalizedStatementRelationStatisticsRepository
    {
        public NormalizedStatementRelationStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
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
    }
}
