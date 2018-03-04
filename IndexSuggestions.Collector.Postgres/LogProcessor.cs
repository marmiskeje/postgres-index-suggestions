﻿using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IndexSuggestions.Collector.Postgres
{
    public class LogProcessor : ILogProcessor
    {
        private readonly List<string> cache = new List<string>();
        private readonly ILogProcessingConfiguration configuration;
        private readonly Regex queryRegex = new Regex(@"{QUERY[\s|\S]*}");
        private readonly Regex planRegex = new Regex(@"{PLANNEDSTMT[\s|\S]*}");

        public LogProcessor(ILogProcessingConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<LoggedEntry> ProcessLine(string line, bool eof)
        {
            List<LoggedEntry> result = new List<LoggedEntry>();
            if (!String.IsNullOrEmpty(line))
            {
                if (line.StartsWith(this.configuration.LogEntryStartingCharacter))
                {
                    if (this.cache.Count > 0)
                    {
                        var tmp = ProcessCache();
                        if (tmp != null)
                        {
                            result.Add(tmp); 
                        }
                    }
                    this.cache.Add(line.Substring(this.configuration.LogEntryStartingCharacter.Length));
                }
                else
                {
                    this.cache.Add(line);
                }
            }
            if (eof && this.cache.Count > 0)
            {
                var tmp = ProcessCache();
                if (tmp != null)
                {
                    result.Add(tmp);
                }
            }
            return result;
        }

        private LoggedEntry ProcessCache()
        {
            LoggedEntry result = null;
            if (this.cache.Count > 0)
            {
                var columns = String.Join("", this.cache).Split(new string[] { "¤" }, StringSplitOptions.None);
                if (columns.Length >= 11)
                {
                    result = new LoggedEntry();
                    result.ApplicationName = columns[2];
                    result.DatabaseName = columns[4];
                    result.ProcessID = columns[1];
                    result.RemoteHostAndPort = columns[5];
                    result.SessionID = columns[6];
                    if (!String.IsNullOrEmpty(columns[7]))
                    {
                        result.SessionLineNumber = long.Parse(columns[7]);
                    }
                    result.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)(double.Parse(columns[0], System.Globalization.CultureInfo.InvariantCulture) * 1000)).LocalDateTime;
                    result.TransactionID = columns[9];
                    result.UserName = columns[3];
                    result.VirtualTransactionIdentifier = columns[8];
                    
                    string input = columns[10];
                    Match match = null;
                    if (input.StartsWith("STATEMENT:"))
                    {
                        result.Statement = input.Substring(10).Trim();
                    }
                    else if ((match = queryRegex.Match(input)) != null && match.Success)
                    {
                        result.QueryTree = match.Value;
                    }
                    else if ((match = planRegex.Match(input)) != null && match.Success)
                    {
                        result.PlanTree = match.Value;
                    }
                }
                this.cache.Clear();
            }
            return result;
        }
    }
}
