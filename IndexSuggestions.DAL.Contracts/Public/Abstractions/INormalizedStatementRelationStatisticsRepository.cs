using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementRelationStatisticsRepository : IBaseRepository<long, NormalizedStatementRelationStatistics>
    {
        NormalizedStatementRelationStatistics GetByUniqueKey(NormalizedStatementRelationStatisticsUniqueKey key);

    }
}
