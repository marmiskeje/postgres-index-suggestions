using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.DAL
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
                return context.WorkloadAnalyses.Where(x => x.State == WorkloadAnalysisStateType.Created).Take(maxCount).ToList();
            }
        }
    }
}
