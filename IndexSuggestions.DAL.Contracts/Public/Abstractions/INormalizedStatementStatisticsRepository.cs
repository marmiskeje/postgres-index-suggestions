using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementStatisticsRepository : IBaseRepository<long, NormalizedStatementStatistics>
    {
        NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key);
        IReadOnlyDictionary<long, List<NormalizedStatementStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo);
        IReadOnlyDictionary<long, NormalizedStatementStatistics> GetTotalGroupedByStatement(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive);
    }
}
