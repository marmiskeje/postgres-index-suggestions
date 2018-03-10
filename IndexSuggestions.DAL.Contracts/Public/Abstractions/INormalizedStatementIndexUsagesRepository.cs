using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementIndexUsagesRepository : IBaseRepository<long, NormalizedStatementIndexUsage>
    {
        NormalizedStatementIndexUsage Get(long statementId, long indexId, DateTime date);

    }
}
