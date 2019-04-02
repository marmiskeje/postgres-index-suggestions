﻿using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class TotalStatisticsCollectNextSampleContext
    {
        public List<IDatabase> Databases { get; } = new List<IDatabase>();
        public Dictionary<uint, ITotalDatabaseStatistics> TotalDatabaseStatistics { get; } = new Dictionary<uint, ITotalDatabaseStatistics>();
    }
}
