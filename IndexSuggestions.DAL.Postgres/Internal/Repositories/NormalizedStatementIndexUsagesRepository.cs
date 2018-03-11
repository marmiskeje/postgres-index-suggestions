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

        public NormalizedStatementIndexUsage Get(long statementId, long indexId, DateTime date, bool useCache = false)
        {
            return Get(() =>
            {
                using (var context = CreateContextFunc())
                {
                    return context.NormalizedStatementIndexUsages.Where(x => x.NormalizedStatementID == statementId && x.IndexID == indexId && x.Date == date).SingleOrDefault();
                }
            }, $"{statementId}{indexId}{date.Ticks}", useCache);
        }

        protected override ISet<string> GetAllCacheKeys(long key, NormalizedStatementIndexUsage entity)
        {
            var result = new HashSet<string>(new[] { $"{entity.NormalizedStatementID}{entity.IndexID}{entity.Date.Ticks}" });
            result.UnionWith(base.GetAllCacheKeys(key, entity));
            return result;
        }
    }
}
