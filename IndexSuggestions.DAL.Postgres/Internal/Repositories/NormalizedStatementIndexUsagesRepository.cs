using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    internal class NormalizedStatementIndexUsagesRepository : BaseRepository<long, NormalizedStatementIndexUsage>, INormalizedStatementIndexUsagesRepository
    {
        public NormalizedStatementIndexUsagesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        public NormalizedStatementIndexUsage Get(long statementId, long indexId, DateTime date)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatementIndexUsages.Where(x => x.NormalizedStatementID == statementId && x.IndexID == indexId && x.Date == date).SingleOrDefault();
            }
        }
    }
}
