using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class WorkloadAnalysisRealStatementEvaluationsRepository : BaseSimpleRepository<WorkloadAnalysisRealStatementEvaluation>, IWorkloadAnalysisRealStatementEvaluationsRepository
    {
        public WorkloadAnalysisRealStatementEvaluationsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        public IEnumerable<WorkloadAnalysisRealStatementEvaluation> GetAllForWorkloadAnalysis(long workloadAnalysisID)
        {
            using (var context = CreateContextFunc())
            {
                return context.WorkloadAnalysisRealStatementEvaluations
                    .Include(x => x.ExecutionPlan).Include(x => x.NormalizedStatement)
                    .Where(x => x.WorkloadAnalysisID == workloadAnalysisID).ToList();
            }
        }
    }
}
