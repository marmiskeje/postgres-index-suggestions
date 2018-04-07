using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class QueryTreeData
    {
        public QueryCommandType QueryCommandType { get; set; }
        public List<QueryTreeRelation> FromTables { get; private set; }
        public QueryTreeData()
        {
            FromTables = new List<QueryTreeRelation>();
        }
    }
    public enum QueryCommandType
    {
        Unknown = 0,
        Select = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        Utility = 5
    }

    public class QueryTreeRelation
    {
        public long ID { get; set; }
    }
}
