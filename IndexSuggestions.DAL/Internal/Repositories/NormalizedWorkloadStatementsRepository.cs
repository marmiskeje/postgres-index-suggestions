using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class NormalizedWorkloadStatementsRepository : BaseRepository<long, NormalizedWorkloadStatement>, INormalizedWorkloadStatementsRepository
    {
        public NormalizedWorkloadStatementsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public NormalizedWorkloadStatement Get(long statementId, long workloadId, bool useCache = false)
        {
            return Get(() =>
            {
                using (var context = CreateContextFunc())
                {
                    return context.NormalizedWorkloadStatements.Where(x => x.NormalizedStatementID == statementId && x.WorkloadID == workloadId).SingleOrDefault();
                }
            }, $"{statementId}{workloadId}", useCache);
        }

        protected override ISet<string> GetAllCacheKeys(long key, NormalizedWorkloadStatement entity)
        {
            var result = new HashSet<string>(new[] { $"{entity.NormalizedStatementID}{entity.WorkloadID}" });
            result.UnionWith(base.GetAllCacheKeys(key, entity));
            return result;
        }
    }
}
