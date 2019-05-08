using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface ITotalStoredProcedureStatisticsRepository : IBaseRepository<long, TotalStoredProcedureStatistics>
    {
        TotalStoredProcedureStatistics GetByUniqueKey(TotalStoredProcedureStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalStoredProcedureStatistics>> GetAllGroupedByProcedure(DateTime createdFrom, DateTime createdTo);
        IEnumerable<TotalStoredProcedureStatistics> GetAllForProcedure(uint storedProcedureID, DateTime dateFromInclusive, DateTime dateToExclusive);
        bool AreDataAvailableForWholePeriod(DateTime dateFrom, DateTime dateTo);
        IReadOnlyDictionary<uint, DateTime> GetForAllLastKnownCollectionDate(uint databaseID);
    }
}
