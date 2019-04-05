using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class TotalViewStatisticsRepository : BaseRepository<long, TotalViewStatistics>, ITotalViewStatisticsRepository
    {
        public TotalViewStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
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
    }
}
