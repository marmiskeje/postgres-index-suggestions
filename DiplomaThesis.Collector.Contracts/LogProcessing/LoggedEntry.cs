using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
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
        public HashSet<string> QueryTrees { get; } = new HashSet<string>();
        public HashSet<string> PlanTrees { get; } = new HashSet<string>();
        public string Detail { get; set; }
    }
}
