/*
 * using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class LoadWorkloadToContextCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public LoadWorkloadToContextCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            var settings = repositories.GetSettingPropertiesRepository();
            var setting = settings.Get(SettingPropertyKeys.ACTIVE_WORKLOAD);
            if (setting != null && setting.IntValue.HasValue)
            {
                var workloads = repositories.GetWorkloadsRepository();
                context.PersistedData.Workload = workloads.GetByPrimaryKey(setting.IntValue.Value, true);
            }
            IsEnabledSuccessorCall = context.PersistedData.Workload != null;
        }
    }
}
*/