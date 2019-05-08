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
        IEnumerable<TotalRelationStatistics> GetAllForRelation(uint relationID, DateTime dateFromInclusive, DateTime dateToExclusive);
        IEnumerable<SummaryTotalRelationStatistics> GetSummaryTotalRelationStatistics(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive, int count);
        bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo);
        IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID);
    }
}
