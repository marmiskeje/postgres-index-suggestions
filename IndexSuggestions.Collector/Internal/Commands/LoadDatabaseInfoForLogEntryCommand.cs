﻿using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class LoadDatabaseInfoForLogEntryCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly LogEntryProcessingContext context;
        private readonly IDatabasesRepository databasesRepository;
        public LoadDatabaseInfoForLogEntryCommand(ILog log, LogEntryProcessingContext context, IDatabasesRepository databasesRepository)
        {
            this.log = log;
            this.context = context;
            this.databasesRepository = databasesRepository;
        }
        protected override void OnExecute()
        {
            var dbInfo = databasesRepository.GetByName(context.Entry.DatabaseName);
            if (dbInfo != null)
            {
                context.DatabaseID = dbInfo.ID;
            }
            else
            {
                log.Write(SeverityType.Warning, "Unknown database with name: {0}. Processing ended.", context.Entry.DatabaseName);
                IsEnabledSuccessorCall = false;
            }
        }
    }
}
