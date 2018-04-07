using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class ApplyQueryTreeWorkloadDefinitionCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        public ApplyQueryTreeWorkloadDefinitionCommand(LogEntryProcessingContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            bool canContinue = context.QueryTree != null;
            if (canContinue)
            {
                var workloadDefinition = context.PersistedData.Workload.Definition;
                List<long> relationIds = new List<long>(); // TODO fill from parse tree
                foreach (var relationId in relationIds)
                {
                    canContinue = canContinue && ApplyWorkloadProperty(relationId, workloadDefinition.Relations.RestrictionType, workloadDefinition.Relations.Values.Select(x => x.ID).ToHashSet());
                } 
            }
            IsEnabledSuccessorCall = canContinue;
        }

        private bool ApplyWorkloadProperty<T>(T logEntryValue, WorkloadPropertyRestrictionType restrictionType, ISet<T> values)
        {
            if (restrictionType == WorkloadPropertyRestrictionType.Allowed)
            {
                return values.Contains(logEntryValue);
            }
            else if (restrictionType == WorkloadPropertyRestrictionType.Disallowed)
            {
                return !values.Contains(logEntryValue);
            }
            return true;
        }
    }
}
