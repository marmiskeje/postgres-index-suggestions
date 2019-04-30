using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalIndexStatisticsRepository : BaseRepository<long, TotalIndexStatistics>, ITotalIndexStatisticsRepository
    {
        public TotalIndexStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
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
    }
}
