using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class NormalizedStatementRelationStatisticsRepository : BaseRepository<long, NormalizedStatementRelationStatistics>, INormalizedStatementRelationStatisticsRepository
    {
        public NormalizedStatementRelationStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
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
