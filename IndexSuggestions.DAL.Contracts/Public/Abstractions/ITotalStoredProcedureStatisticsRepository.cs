using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface ITotalStoredProcedureStatisticsRepository : IBaseRepository<long, TotalStoredProcedureStatistics>
    {
        TotalStoredProcedureStatistics GetByUniqueKey(TotalStoredProcedureStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalStoredProcedureStatistics>> GetAllGroupedByProcedure(DateTime createdFrom, DateTime createdTo);
    }
}
