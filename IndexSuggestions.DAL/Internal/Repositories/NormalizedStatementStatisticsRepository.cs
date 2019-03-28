using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class NormalizedStatementStatisticsRepository : BaseRepository<long, NormalizedStatementStatistics>, INormalizedStatementStatisticsRepository
    {
        public NormalizedStatementStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IReadOnlyDictionary<long, List<NormalizedStatementStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.NormalizedStatementID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementStatistics.Where(x => x.ApplicationName == key.ApplicationName
                        && x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.NormalizedStatementID == key.NormalizedStatementID
                        && x.UserName == key.UserName).SingleOrDefault();
            }
        }
    }
}
