﻿using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class CollectTotalDatabaseStatisticsCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly TotalStatisticsCollectNextSampleContext context;
        private readonly ITotalDatabaseStatisticsRepository repository;
        public CollectTotalDatabaseStatisticsCommand(ILog log, TotalStatisticsCollectNextSampleContext context, ITotalDatabaseStatisticsRepository repository)
        {
            this.log = log;
            this.context = context;
            this.repository = repository;
        }
        protected override void OnExecute()
        {
            foreach (var database in context.Databases)
            {
                using (DatabaseScope scope = new DatabaseScope(database.Name))
                {
                    try
                    {
                        context.TotalDatabaseStatistics.Add(database.ID, repository.GetLatest());
                    }
                    catch (Exception ex)
                    {
                        log.Write(ex);
                    }
                }
            }
        }
    }
}
