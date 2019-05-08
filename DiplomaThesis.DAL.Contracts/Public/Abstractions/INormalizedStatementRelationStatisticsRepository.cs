using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedStatementRelationStatisticsRepository : IBaseRepository<long, NormalizedStatementRelationStatistics>
    {
        NormalizedStatementRelationStatistics GetByUniqueKey(NormalizedStatementRelationStatisticsUniqueKey key);
        IReadOnlyDictionary<long, List<NormalizedStatementRelationStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo);
        IEnumerable<RelationSummaryStatementStatistics> GetRelationSummaryStatementStatistics(uint relationID, DateTime dateFromInclusive, DateTime dateToExclusive);
        IEnumerable<NormalizedStatementRelationStatistics> GetAllForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive);
    }
}
