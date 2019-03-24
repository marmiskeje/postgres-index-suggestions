using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementStatisticsRepository : IBaseRepository<long, NormalizedStatementStatistics>
    {
        NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key);
    }
}
