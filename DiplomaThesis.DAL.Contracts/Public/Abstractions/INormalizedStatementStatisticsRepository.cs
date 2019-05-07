using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedStatementStatisticsRepository : IBaseRepository<long, NormalizedStatementStatistics>
    {
        NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key);
        IReadOnlyDictionary<long, List<NormalizedStatementStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo);
        IEnumerable<SummaryNormalizedStatementStatistics> GetSummaryTotalStatementStatistics(uint databaseID, DateTime dateFromInclusive, DateTime dateToExclusive, SummaryNormalizedStatementStatisticsOrderBy orderBy, int count);
    }

    public enum SummaryNormalizedStatementStatisticsOrderBy
    {
        ExecutionCount = 0,
        MaxDuration = 1
    }
}
