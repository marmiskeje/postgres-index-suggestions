using DiplomaThesis.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class LogEntryProcessingWrapperContext
    {
        public LogEntryProcessingContext InnerContext { get; set; }
        public List<JObject> QueryPlans { get; } = new List<JObject>();
        public List<JObject> QueryTrees { get; } = new List<JObject>();
    }
}
