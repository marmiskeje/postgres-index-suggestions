using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class VirtualEnvironmentStatementEvaluationsRepository : BaseSimpleRepository<VirtualEnvironmentStatementEvaluation>, IVirtualEnvironmentStatementEvaluationsRepository
    {
        public VirtualEnvironmentStatementEvaluationsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
