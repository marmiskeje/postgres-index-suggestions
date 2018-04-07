using IndexSuggestions.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal class LogEntryProcessingWrapperContext
    {
        public LogEntryProcessingContext InnerContext { get; set; }
        public JObject QueryPlan { get; set; }
        public JObject QueryTree { get; set; }
    }
}
