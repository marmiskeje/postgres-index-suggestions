using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class LogEntryProcessingContext
    {
        public LoggedEntry Entry { get; set; }
        public string NormalizedStatement { get; set; }
        public QueryPlanNode QueryPlan { get; set; }
    }
}
