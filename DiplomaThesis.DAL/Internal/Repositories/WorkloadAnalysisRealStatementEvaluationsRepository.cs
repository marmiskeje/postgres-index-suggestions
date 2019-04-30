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
    }
}
