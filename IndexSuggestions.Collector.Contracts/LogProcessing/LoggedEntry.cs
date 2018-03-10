using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class LoggedEntry
    {
        public DateTime Timestamp { get; set; }
        public string ProcessID { get; set; }
        public string ApplicationName { get; set; }
        public string UserName { get; set; }
        public string DatabaseName { get; set; }
        public string RemoteHostAndPort { get; set; }
        public string SessionID { get; set; }
        public long SessionLineNumber { get; set; }
        public string VirtualTransactionIdentifier { get; set; }
        public string TransactionID { get; set; }
        public TimeSpan Duration { get; set; }
        public string Statement { get; set; }
        public string QueryTree { get; set; }
        public string PlanTree { get; set; }
        public string Detail { get; set; }
    }
}
