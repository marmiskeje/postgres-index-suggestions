﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface ITotalViewStatisticsRepository : IBaseRepository<long, TotalViewStatistics>
    {
        TotalViewStatistics GetByUniqueKey(TotalViewStatisticsUniqueKey key);
        IReadOnlyDictionary<uint, List<TotalViewStatistics>> GetAllGroupedByView(DateTime createdFrom, DateTime createdTo);
    }
}