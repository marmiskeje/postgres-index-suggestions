using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class WorkloadsRepository : BaseRepository<long, Workload>, IWorkloadsRepository
    {
        public WorkloadsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            CacheExpiration = TimeSpan.FromMinutes(5);
        }

        protected override Workload GetByIdLoadFromDb(long id)
        {
            var result = base.GetByIdLoadFromDb(id);
            result.Definition = JsonSerializationUtility.Deserialize<WorkloadDefinition>(result.DefinitionData);
            return result;
        }
    }
}
