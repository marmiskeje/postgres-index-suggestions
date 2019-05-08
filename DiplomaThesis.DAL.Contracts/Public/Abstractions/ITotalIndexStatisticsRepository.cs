using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface ITotalIndexStatisticsRepository : IBaseRepository<long, TotalIndexStatistics>
    {
        TotalIndexStatistics GetByUniqueKey(TotalIndexStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalIndexStatistics>> GetAllGroupedByIndex(DateTime createdFrom, DateTime createdTo);
        IEnumerable<TotalIndexStatistics> GetAllForIndex(uint indexID, DateTime dateFromInclusive, DateTime dateToExclusive);
        bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo);
        IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID);
    }
}
