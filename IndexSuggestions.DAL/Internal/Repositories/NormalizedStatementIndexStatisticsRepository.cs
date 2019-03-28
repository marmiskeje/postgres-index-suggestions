using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class NormalizedStatementIndexStatisticsRepository : BaseRepository<long, NormalizedStatementIndexStatistics>, INormalizedStatementIndexStatisticsRepository
    {
        public NormalizedStatementIndexStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
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
    }
}
