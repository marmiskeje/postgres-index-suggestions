using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface ITotalRelationStatisticsRepository : IBaseRepository<long, TotalRelationStatistics>
    {
        TotalRelationStatistics GetByUniqueKey(TotalRelationStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalRelationStatistics>> GetAllGroupedByRelation(DateTime createdFrom, DateTime createdTo);
    }
}
