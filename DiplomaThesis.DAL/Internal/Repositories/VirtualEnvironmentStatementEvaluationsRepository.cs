using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentStatementEvaluationsRepository : BaseSimpleRepository<VirtualEnvironmentStatementEvaluation>, IVirtualEnvironmentStatementEvaluationsRepository
    {
        public VirtualEnvironmentStatementEvaluationsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
