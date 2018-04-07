using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class LogEntryProcessingContext
    {
        public LoggedEntry Entry { get; set; }
        public string NormalizedStatement { get; set; }
        public string NormalizedStatementFingerprint { get; set; }
        public QueryPlanNode QueryPlan { get; set; }
        public LogEntryProcessingContextPersistedData PersistedData { get; private set; }
        public LogEntryProcessingContext()
        {
            PersistedData = new LogEntryProcessingContextPersistedData();
        }
    }

    public class LogEntryProcessingContextPersistedData
    {
        public NormalizedStatement NormalizedStatement { get; set; }
        public Workload Workload { get; set; }
    }
}
