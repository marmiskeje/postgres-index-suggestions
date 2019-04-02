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

        public IReadOnlyDictionary<uint, List<TotalRelationStatistics>> GetAllGroupedByRelation(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalRelationStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
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
    }
}
