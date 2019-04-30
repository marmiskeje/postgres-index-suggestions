using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    /// <summary>
    /// We need to ignore own log entries otherwise infinite log processing can occur.
    /// </summary>
    internal class IgnoreOwnLogEntriesCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        public IgnoreOwnLogEntriesCommand(LogEntryProcessingContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            this.IsEnabledSuccessorCall = context.Entry.ApplicationName != "IndexSuggestions";
        }
    }
}
