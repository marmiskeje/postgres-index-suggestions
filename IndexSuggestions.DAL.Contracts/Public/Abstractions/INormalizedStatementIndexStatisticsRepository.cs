using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementIndexStatisticsRepository : IBaseRepository<long, NormalizedStatementIndexStatistics>
    {
        NormalizedStatementIndexStatistics GetByUniqueKey(NormalizedStatementIndexStatisticsUniqueKey key);

    }
}
