using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface ITotalIndexStatisticsRepository : IBaseRepository<long, TotalIndexStatistics>
    {
        TotalIndexStatistics GetByUniqueKey(TotalIndexStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalIndexStatistics>> GetAllGroupedByIndex(DateTime createdFrom, DateTime createdTo);
    }
}
