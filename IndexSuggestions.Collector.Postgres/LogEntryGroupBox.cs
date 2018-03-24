using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace IndexSuggestions.Collector.Postgres
{
    public class LogEntryGroupBox : ILogEntryGroupBox
    {
        #region private class LoggedEntryGroup
        private class LoggedEntryGroup
        {
            private readonly List<LoggedEntry> entries = new List<LoggedEntry>();
            private DateTime LastAccess { get; set; }
            public bool IsDirty
            {
                get { return LastAccess.AddSeconds(2) <= DateTime.Now; }
            }
            public void Add(LoggedEntry entry)
            {
                entries.Add(entry);
                LastAccess = DateTime.Now;
            }

            public IEnumerable<LoggedEntry> Provide()
            {
                List<LoggedEntry> result = new List<LoggedEntry>();
                LoggedEntry currentResult = new LoggedEntry();
                foreach (var e in entries)
                {
                    FillResult(ref currentResult, e);
                    if (!String.IsNullOrWhiteSpace(currentResult.Statement))
                    {
                        result.Add(currentResult);
                        currentResult = new LoggedEntry();
                    }
                }
                if (!result.Contains(currentResult) && !String.IsNullOrWhiteSpace(currentResult.Statement))
                {
                    result.Add(currentResult);
                }
                return result.OrderBy(x => x.Timestamp);
            }

            private void FillResult(ref LoggedEntry result, LoggedEntry e)
            {
                if (!String.IsNullOrEmpty(e.ApplicationName))
                {
                    result.ApplicationName = e.ApplicationName;
                }
                if (!String.IsNullOrEmpty(e.DatabaseName))
                {
                    result.DatabaseName = e.DatabaseName;
                }
                if (e.Duration.TotalMilliseconds > 0)
                {
                    result.Duration = e.Duration;
                }
                if (!String.IsNullOrEmpty(e.PlanTree))
                {
                    result.PlanTree = e.PlanTree;
                }
                if (!String.IsNullOrEmpty(e.ProcessID))
                {
                    result.ProcessID = e.ProcessID;
                }
                if (!String.IsNullOrEmpty(e.QueryTree))
                {
                    result.QueryTree = e.QueryTree;
                }
                if (!String.IsNullOrEmpty(e.RemoteHostAndPort))
                {
                    result.RemoteHostAndPort = e.RemoteHostAndPort;
                }
                if (!String.IsNullOrEmpty(e.SessionID))
                {
                    result.SessionID = e.SessionID;
                }
                if (!String.IsNullOrEmpty(e.Statement))
                {
                    result.Statement = e.Statement;
                }
                if (result.Timestamp == DateTime.MinValue)
                {
                    result.Timestamp = e.Timestamp;
                }
                if (!String.IsNullOrEmpty(e.TransactionID))
                {
                    result.TransactionID = e.TransactionID;
                }
                if (!String.IsNullOrEmpty(e.UserName))
                {
                    result.UserName = e.UserName;
                }
            }
        } 
        #endregion
        private readonly Dictionary<string, LoggedEntryGroup> groups = new Dictionary<string, LoggedEntryGroup>();
        private readonly List<LoggedEntry> fullEntries = new List<LoggedEntry>();
        private readonly object lockObject = new object();
        private readonly Timer timer;
        private bool isDisposed = false;
        public LogEntryGroupBox()
        {
            timer = new Timer(Timer_Job);
            timer.Change(2000, 2000);
        }
        public IEnumerable<LoggedEntry> Provide()
        {
            lock (lockObject)
            {
                List<LoggedEntry> result = new List<LoggedEntry>(fullEntries.Where(x => x.Statement != null));
                fullEntries.Clear();
                return result;
            }
        }

        public void Publish(IEnumerable<LoggedEntry> entries)
        {
            lock (lockObject)
            {
                foreach (var entry in entries)
                {
                    var key = CreateUniqueKey(entry);
                    if (!groups.ContainsKey(key))
                    {
                        groups.Add(key, new LoggedEntryGroup());
                    }
                    groups[key].Add(entry);
                }
            }
        }

        private string CreateUniqueKey(LoggedEntry entry)
        {
            return $"{entry.ApplicationName}_{entry.ProcessID}_{entry.RemoteHostAndPort}_{entry.SessionID}_{entry.Timestamp}_{entry.UserName}_{entry.VirtualTransactionIdentifier}";
        }

        private void Timer_Job(object obj)
        {
            List<string> dirtyKeys = new List<string>();
            lock (lockObject)
            {
                foreach (var g in groups)
                {
                    if (g.Value.IsDirty)
                    {
                        dirtyKeys.Add(g.Key);
                    }
                }
                foreach (var key in dirtyKeys)
                {
                    LoggedEntryGroup group = null;
                    if (groups.Remove(key, out group))
                    {
                        fullEntries.AddRange(group.Provide());
                    }
                }
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                }
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LogEntryGroupBox()
        {
            Dispose(false);
        }
    }
}
