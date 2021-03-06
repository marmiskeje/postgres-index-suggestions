﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedStatementIndexStatisticsRepository : IBaseRepository<long, NormalizedStatementIndexStatistics>
    {
        NormalizedStatementIndexStatistics GetByUniqueKey(NormalizedStatementIndexStatisticsUniqueKey key);
        IReadOnlyDictionary<long, List<NormalizedStatementIndexStatistics>> GetAllGroupedByStatement(DateTime createdFrom, DateTime createdTo);
        IEnumerable<IndexSummaryStatementStatistics> GetIndexSummaryStatementStatistics(uint indexID, DateTime dateFromInclusive, DateTime dateToExclusive);
        IEnumerable<NormalizedStatementIndexStatistics> GetAllForStatement(long normalizedStatementID, DateTime dateFromInclusive, DateTime dateToExclusive);
    }
}
