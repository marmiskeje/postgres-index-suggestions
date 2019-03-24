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
