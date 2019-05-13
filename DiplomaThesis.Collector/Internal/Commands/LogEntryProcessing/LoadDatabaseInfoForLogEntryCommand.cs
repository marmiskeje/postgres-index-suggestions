using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class LoadDatabaseInfoForLogEntryCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly LogEntryProcessingContext context;
        private readonly IDatabasesRepository databasesRepository;
        private readonly DAL.Contracts.ISettingPropertiesRepository settingPropertiesRepository;
        public LoadDatabaseInfoForLogEntryCommand(ILog log, LogEntryProcessingContext context, IDatabasesRepository databasesRepository,
                                                  DAL.Contracts.ISettingPropertiesRepository settingPropertiesRepository)
        {
            this.log = log;
            this.context = context;
            this.databasesRepository = databasesRepository;
            this.settingPropertiesRepository = settingPropertiesRepository;
        }
        protected override void OnExecute()
        {
            var dbInfo = databasesRepository.GetByName(context.Entry.DatabaseName);
            if (dbInfo != null)
            {
                context.DatabaseID = dbInfo.ID;
                var collectorConfiguration = settingPropertiesRepository.GetObject<DAL.Contracts.CollectorConfiguration>(DAL.Contracts.SettingPropertyKeys.COLLECTOR_CONFIGURATION, true);
                if (collectorConfiguration != null && collectorConfiguration.Databases.ContainsKey(dbInfo.ID))
                {
                    context.DatabaseCollectingConfiguration = collectorConfiguration.Databases[dbInfo.ID];
                }
                else
                {
                    context.DatabaseCollectingConfiguration = new DAL.Contracts.CollectorDatabaseConfiguration() { DatabaseID = dbInfo.ID };
                }
            }
            else
            {
                log.Write(SeverityType.Warning, "Unknown database with name: {0}. Processing ended.", context.Entry.DatabaseName);
                IsEnabledSuccessorCall = false;
            }
        }
    }
}
