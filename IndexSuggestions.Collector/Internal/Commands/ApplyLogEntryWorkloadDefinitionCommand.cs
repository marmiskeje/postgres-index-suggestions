/*
using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class ApplyLogEntryWorkloadDefinitionCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        public ApplyLogEntryWorkloadDefinitionCommand(LogEntryProcessingContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            bool canContinue = true;
            var workloadDefinition = context.PersistedData.Workload.Definition;
            if (workloadDefinition.Applications != null)
            {
                canContinue = canContinue && ApplyWorkloadProperty(context.Entry.ApplicationName, workloadDefinition.Applications);
            }
            if (!String.IsNullOrEmpty(workloadDefinition.DatabaseName))
            {
                canContinue = canContinue && context.Entry.DatabaseName == workloadDefinition.DatabaseName;
            }
            if (workloadDefinition.DateTimeSlots != null)
            {
                bool initTmpContinue = workloadDefinition.DateTimeSlots.RestrictionType == WorkloadPropertyRestrictionType.Allowed ? false : true;

                bool tmpCanContinue = initTmpContinue;
                var date = context.Entry.Timestamp;
                foreach (var s in workloadDefinition.DateTimeSlots.Values)
                {
                    if (date.DayOfWeek == s.DayOfWeek && date.TimeOfDay >= s.StartTime && date.TimeOfDay <= s.EndTime)
                    {
                        tmpCanContinue = !initTmpContinue;
                        break;
                    }
                }
                canContinue = canContinue && tmpCanContinue;
            }
            if (workloadDefinition.QueryThresholds != null)
            {
                if (workloadDefinition.QueryThresholds.MinDuration.HasValue)
                {
                    canContinue = canContinue && context.Entry.Duration >= workloadDefinition.QueryThresholds.MinDuration.Value; 
                }
            }
            if (workloadDefinition.Users != null)
            {
                canContinue = canContinue && ApplyWorkloadProperty(context.Entry.UserName, workloadDefinition.Users);
            }
            IsEnabledSuccessorCall = canContinue;
        }

        private bool ApplyWorkloadProperty<T>(T logEntryValue, WorkloadPropertyValuesDefinition<T> workloadPropertyDefinition)
        {
            if (workloadPropertyDefinition.RestrictionType == WorkloadPropertyRestrictionType.Allowed)
            {
                return workloadPropertyDefinition.Values.Contains(logEntryValue);
            }
            else if (workloadPropertyDefinition.RestrictionType == WorkloadPropertyRestrictionType.Disallowed)
            {
                return !workloadPropertyDefinition.Values.Contains(logEntryValue);
            }
            return true;
        }
    }
}
*/