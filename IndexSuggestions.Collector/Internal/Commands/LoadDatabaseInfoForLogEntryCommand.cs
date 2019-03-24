using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class LoadDatabaseInfoForLogEntryCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IDatabasesRepository databasesRepository;
        public LoadDatabaseInfoForLogEntryCommand(LogEntryProcessingContext context, IDatabasesRepository databasesRepository)
        {
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
            IsEnabledSuccessorCall = dbInfo != null;
        }
    }
}
