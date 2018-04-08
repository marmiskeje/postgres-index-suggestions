using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class QueryTreeData
    {
        public QueryCommandType QueryCommandType { get; set; }
        public List<QueryTreeRelation> Relations { get; private set; }
        public List<QueryTreePredicate> Predicates { get; private set; }
        public QueryTreeData()
        {
            Relations = new List<QueryTreeRelation>();
            Predicates = new List<QueryTreePredicate>();
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

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is QueryTreeRelation))
            {
                return false;
            }
            QueryTreeRelation tmp = (QueryTreeRelation)obj;
            return ID.Equals(tmp.ID);
        }
    }

    public class QueryTreePredicate
    {
        public long OperatorID { get; set; }
        public List<QueryTreePredicateOperand> Operands { get; private set; }
        public QueryTreePredicate()
        {
            Operands = new List<QueryTreePredicateOperand>();
        }
    }

    public class QueryTreePredicateOperand
    {
        public DbType Type { get; set; }
        public long TypeId { get; set; }
        public long? RelationID { get; set; }
        public string AttributeName { get; set; }
        public dynamic ConstValue { get; set; }
    }
}
