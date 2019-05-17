using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class WorkloadsRepository : BaseRepository<long, Workload>, IWorkloadsRepository
    {
        public WorkloadsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
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

        public IEnumerable<Workload> GetForDatabase(uint databaseID, DateTime createdDateFromInclusive, DateTime createdDateToExlusive, bool onlyActive = true)
        {
            using (var context = CreateContextFunc())
            {
                var query = context.Workloads.Where(x => x.DatabaseID == databaseID && x.CreatedDate >= createdDateFromInclusive && x.CreatedDate < createdDateToExlusive);
                if (onlyActive)
                {
                    query = query.Where(x => x.InactiveDate == null);
                }
                var result = query.ToList();
                result.ForEach(x => FillEntityGet(x));
                return result;
            }
        }
    }
}
