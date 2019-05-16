using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class WorkloadAnalysesRepository : BaseRepository<long, WorkloadAnalysis>, IWorkloadAnalysesRepository
    {
        public WorkloadAnalysesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
        public IEnumerable<WorkloadAnalysis> LoadForProcessing(int maxCount)
        {
            using (var context = CreateContextFunc())
            {
                var result = context.WorkloadAnalyses.Where(x => x.State == WorkloadAnalysisStateType.Created).Take(maxCount).ToList();
                result.ForEach(x => FillEntityGet(x));
                return result;
            }
        }

        protected override void FillEntitySet(WorkloadAnalysis entity)
        {
            base.FillEntitySet(entity);
            entity.RelationReplacementsData = JsonSerializationUtility.Serialize(entity.RelationReplacements);
        }

        protected override void FillEntityGet(WorkloadAnalysis entity)
        {
            base.FillEntityGet(entity);
            if (entity.RelationReplacementsData != null)
            {
                entity.RelationReplacements = JsonSerializationUtility.Deserialize<List<WorkloadAnalysisRelationReplacement>>(entity.RelationReplacementsData); 
            }
        }

        public IEnumerable<WorkloadAnalysis> GetForDatabase(uint databaseID, DateTime createdDateFromInclusive, DateTime createdDateToExlusive)
        {
            using (var context = CreateContextFunc())
            {
                return context.WorkloadAnalyses.Include(x => x.Workload).Where(x => x.Workload.DatabaseID == databaseID && x.CreatedDate >= createdDateFromInclusive && x.CreatedDate < createdDateToExlusive)
                            .OrderByDescending(x => x.CreatedDate).ToList();
            }
        }
    }
}
