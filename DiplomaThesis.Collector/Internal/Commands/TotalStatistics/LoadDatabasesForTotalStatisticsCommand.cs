using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class LoadDatabasesForTotalStatisticsCommand : ChainableCommand
    {
        private readonly TotalStatisticsCollectNextSampleContext context;
        private readonly IDatabasesRepository databasesRepository;
        private readonly DAL.Contracts.ISettingPropertiesRepository settingPropertiesRepository;
        public LoadDatabasesForTotalStatisticsCommand(TotalStatisticsCollectNextSampleContext context, IDatabasesRepository databasesRepository,
                                                      DAL.Contracts.ISettingPropertiesRepository settingPropertiesRepository)
        {
            this.context = context;
            this.databasesRepository = databasesRepository;
            this.settingPropertiesRepository = settingPropertiesRepository;
        }
        protected override void OnExecute()
        {
            var configuration = settingPropertiesRepository.GetObject<DAL.Contracts.CollectorConfiguration>(DAL.Contracts.SettingPropertyKeys.COLLECTOR_CONFIGURATION);
            if (configuration != null)
            {
                foreach (var kv in configuration.Databases)
                {
                    var databaseID = kv.Key;
                    var databaseCollectorConfiguration = kv.Value;
                    if (databaseCollectorConfiguration.IsEnabledGeneralCollection)
                    {
                        var db = databasesRepository.Get(databaseID);
                        if (db != null)
                        {
                            context.Databases.Add(db);
                        }
                    }
                }
            }
        }
    }
}
