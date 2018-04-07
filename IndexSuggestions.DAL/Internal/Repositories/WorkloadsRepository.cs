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

        protected override void FillEntitySet(Workload entity)
        {
            base.FillEntitySet(entity);
            entity.DefinitionData = JsonSerializationUtility.Serialize(entity.Definition);
        }

        protected override void FillEntityGet(Workload entity)
        {
            base.FillEntityGet(entity);
            entity.Definition = JsonSerializationUtility.Deserialize<WorkloadDefinition>(entity.DefinitionData);
        }
    }
}
