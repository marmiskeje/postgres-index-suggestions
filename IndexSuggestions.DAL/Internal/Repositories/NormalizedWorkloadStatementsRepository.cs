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

        public IList<NormalizedWorkloadStatement> GetAllByWorkloadId(long workloadId, NormalizedWorkloadStatementFilter filter)
        {
            List<NormalizedWorkloadStatement> result = null;
            filter = filter ?? new NormalizedWorkloadStatementFilter();
            using (var context = CreateContextFunc())
            {
                var query = context.NormalizedWorkloadStatements.Include(x => x.NormalizedStatement).Where(x => x.WorkloadID == workloadId);
                if (filter.CommandType.HasValue)
                {
                    query = query.Where(x => x.NormalizedStatement.CommandType == filter.CommandType.Value);
                }
                if (filter.MinExecutionsCount.HasValue)
                {
                    query = query.Where(x => x.ExecutionsCount >= filter.MinExecutionsCount.Value);
                }
                result = query.ToList();
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
