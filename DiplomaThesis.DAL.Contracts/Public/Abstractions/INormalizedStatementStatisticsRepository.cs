using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedStatementStatisticsRepository : IBaseRepository<long, NormalizedStatementStatistics>
    {
        NormalizedStatementStatistics GetByUniqueKey(NormalizedStatementStatisticsUniqueKey key);
        IReadOnlyDictionary<long, List<NormalizedStatementStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo);
        IEnumerable<SummaryNormalizedStatementStatistics> GetSummaryTotalStatementStatistics(uint databaseID,
                        DateTime dateFromInclusive, DateTime dateToExclusive, SummaryNormalizedStatementStatisticsOrderBy orderBy,
                        StatementQueryCommandType? commandType, int? count);
        IEnumerable<NormalizedStatementStatisticsTimeline> GetTimelineForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive);
        IEnumerable<NormalizedStatementStatistics> GetSlowestForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive, int count);
    }

    public enum SummaryNormalizedStatementStatisticsOrderBy
    {
        ExecutionCount = 0,
        MaxDuration = 1
    }

    public class NormalizedStatementStatisticsTimeline
    {
        public DateTime Date { get; set; }
        public long TotalExecutionsCount { get; set; }
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxDuration { get; set; }
        public TimeSpan AvgDuration { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public decimal? MinTotalCost { get; set; }
        public decimal? MaxTotalCost { get; set; }
        public decimal? AvgTotalCost { get; set; }
    }
}
