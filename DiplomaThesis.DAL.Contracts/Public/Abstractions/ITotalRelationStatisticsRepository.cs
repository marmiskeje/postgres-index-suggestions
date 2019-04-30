using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface ITotalRelationStatisticsRepository : IBaseRepository<long, TotalRelationStatistics>
    {
        TotalRelationStatistics GetByUniqueKey(TotalRelationStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalRelationStatistics>> GetAllGroupedByRelation(DateTime createdFromInclusive, DateTime createdToExclusive);
        IEnumerable<TotalRelationStatistics> GetAllForDatabase(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive);
        IReadOnlyDictionary<uint, TotalRelationStatistics> GetTotalGroupedByRelation(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive);
    }
}
