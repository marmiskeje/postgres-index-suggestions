using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

        public IList<NormalizedWorkloadStatement> GetAllByWorkloadId(long workloadId)
        {
            List<NormalizedWorkloadStatement> result = null;
            using (var context = CreateContextFunc())
            {
                result = context.NormalizedWorkloadStatements.Where(x => x.WorkloadID == workloadId).Include(x => x.NormalizedStatement).ToList();
            }
            result.ForEach(x =>
            {
                if (x.NormalizedStatement.StatementDefinitionData != null)
                {
                    x.NormalizedStatement.StatementDefinition = JsonSerializationUtility.Deserialize<StatementDefinition>(x.NormalizedStatement.StatementDefinitionData);
                }
            });
            return result;
        }

        protected override ISet<string> GetAllCacheKeys(long key, NormalizedWorkloadStatement entity)
        {
            var result = new HashSet<string>(new[] { $"{entity.NormalizedStatementID}{entity.WorkloadID}" });
            result.UnionWith(base.GetAllCacheKeys(key, entity));
            return result;
        }
    }
}
