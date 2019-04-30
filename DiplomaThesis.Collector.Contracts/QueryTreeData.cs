using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public class QueryTreeData
    {
        public QueryCommandType CommandType { get; set; }
        public List<QueryData> IndependentQueries { get; private set; }
        public QueryTreeData()
        {
            IndependentQueries = new List<QueryData>();
        }
    }
    public class QueryData
    {
        public QueryCommandType CommandType { get; set; }
        public ISet<QueryTreeRelation> Relations { get; private set; }
        public ISet<QueryTreeAttribute> ProjectionAttributes { get; private set; }
        public IList<QueryTreeExpression> WhereExpressions { get; private set; }
        public IList<QueryTreeExpression> JoinExpressions { get; private set; }
        public IList<QueryTreeExpression> HavingExpressions { get; private set; }
        public IList<QueryTreeExpression> OrderByExpressions { get; private set; }
        public IList<QueryTreeExpression> GroupByExpressions { get; private set; }
        public QueryData()
        {
            Relations = new HashSet<QueryTreeRelation>();
            ProjectionAttributes = new HashSet<QueryTreeAttribute>();
            WhereExpressions = new List<QueryTreeExpression>();
            JoinExpressions = new List<QueryTreeExpression>();
            HavingExpressions = new List<QueryTreeExpression>();
            OrderByExpressions = new List<QueryTreeExpression>();
            GroupByExpressions = new List<QueryTreeExpression>();
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
        public uint ID { get; set; }

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

    public class QueryTreeAttribute
    {
        public int AttributeNumber { get; set; }
        public uint RelationID { get; set; }

        public override int GetHashCode()
        {
            return $"{RelationID}_{AttributeNumber}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is QueryTreeAttribute))
            {
                return false;
            }
            QueryTreeAttribute tmp = (QueryTreeAttribute)obj;
            return RelationID.Equals(tmp.RelationID) && AttributeNumber.Equals(tmp.AttributeNumber);
        }
    }

    public abstract class QueryTreeExpression
    {
    }

    public class QueryTreeFunctionExpression : QueryTreeExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public List<QueryTreeExpression> Arguments { get; private set; }
        public QueryTreeFunctionExpression()
        {
            Arguments = new List<QueryTreeExpression>();
        }
    }

    public class QueryTreeUnknownExpression : QueryTreeExpression
    {
    }

    public class QueryTreeAttributeExpression : QueryTreeExpression
    {
        public uint RelationID { get; set; }
        public int AttributeNumber { get; set; }
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
    }

    public class QueryTreeConstExpression : QueryTreeExpression
    {
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
    }
    public class QueryTreeBooleanExpression : QueryTreeExpression
    {
        public string Operator { get; set; }
        public List<QueryTreeExpression> Arguments { get; private set; }
        public QueryTreeBooleanExpression()
        {
            Arguments = new List<QueryTreeExpression>();
        }
    }
    public class QueryTreeOperatorExpression : QueryTreeExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public uint OperatorID { get; set; }
        public List<QueryTreeExpression> Arguments { get; private set; }
        public QueryTreeOperatorExpression()
        {
            Arguments = new List<QueryTreeExpression>();
        }
    }

    public class QueryTreeNullTestExpression : QueryTreeExpression
    {
        public QueryTreeNullTestType TestType { get; set; }
        public QueryTreeExpression Argument { get; set; }
    }

    public enum QueryTreeNullTestType
    {
        Unkown = 0,
        IsNull = 1,
        IsNotNull = 2
    }
}
