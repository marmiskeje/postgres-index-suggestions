using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedWorkloadStatementsRepository : IBaseRepository<long, NormalizedWorkloadStatement>
    {
        NormalizedWorkloadStatement Get(long statementId, long workloadId, bool useCache = false);

    }
}
