using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal abstract class EvalExpression
    {
        public QueryTreeDataInternal Source { get; private set; }

        public EvalExpression(QueryTreeDataInternal source)
        {
            Source = source;
        }
    }
    internal class UnknownExpression : EvalExpression
    {
        public UnknownExpression(QueryTreeDataInternal source) : base(source)
        {

        }
    }
    internal class ConstExpression : EvalExpression
    {
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
        public ConstExpression(QueryTreeDataInternal source) : base(source)
        {

        }
    }
    internal class FunctionExpression : EvalExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public List<EvalExpression> Arguments { get; private set; }
        public FunctionExpression(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<EvalExpression>();
        }
    }
    internal class VariableExpression : EvalExpression
    {
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
        public int RteNumber { get; private set; }
        public int Position { get; private set; }
        public VariableExpression(QueryTreeDataInternal source, int rteNumber, int position) : base(source)
        {
            RteNumber = rteNumber;
            Position = position;
        }
    }
    internal class SublinkExpression : EvalExpression
    {
        public QueryTreeDataInternal SubQuery { get; set; }
        public SublinkExpression(QueryTreeDataInternal source) : base(source)
        {

        }
    }

    internal class BooleanExpression : EvalExpression
    {
        public string Operator { get; set; }
        public List<EvalExpression> Arguments { get; private set; }
        public BooleanExpression(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<EvalExpression>();
        }
    }
    internal class OperatorExpression : EvalExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public uint OperatorID { get; set; }
        public List<EvalExpression> Arguments { get; private set; }
        public OperatorExpression(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<EvalExpression>();
        }
    }

    internal class NullTestExpression : EvalExpression
    {
        public NullTestType TestType { get; set; }
        public EvalExpression Argument { get; set; }
        public NullTestExpression(QueryTreeDataInternal source) : base(source)
        {
            TestType = NullTestType.Unknown;
        }
    }

    internal abstract class ExpressionResult
    {
        public QueryTreeDataInternal Source { get; private set; }

        public ExpressionResult(QueryTreeDataInternal source)
        {
            Source = source;
        }

        public abstract ISet<QueryTreeRelation> GetAllRelations();
        public abstract ISet<QueryTreeAttribute> GetAllAttributes();
    }

    internal class FunctionExpressionResult : ExpressionResult
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public List<ExpressionResult> Arguments { get; private set; }
        public FunctionExpressionResult(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<ExpressionResult>();
        }

        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            var result = new HashSet<QueryTreeRelation>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllRelations());
            }
            return result;
        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            var result = new HashSet<QueryTreeAttribute>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllAttributes());
            }
            return result;
        }
    }

    internal class UnknownExpressionResult : ExpressionResult
    {
        public UnknownExpressionResult(QueryTreeDataInternal source) : base(source)
        {

        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            return new HashSet<QueryTreeAttribute>();
        }

        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            return new HashSet<QueryTreeRelation>();
        }
    }

    internal class AttributeExpressionResult : ExpressionResult
    {
        public uint RelationID { get; set; }
        public int AttributeNumber { get; set; }
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
        public AttributeExpressionResult(QueryTreeDataInternal source) : base(source)
        {

        }

        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            var result = new HashSet<QueryTreeRelation>();
            result.Add(new QueryTreeRelation() { ID = RelationID });
            return result;
        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            var result = new HashSet<QueryTreeAttribute>();
            result.Add(new QueryTreeAttribute() { RelationID = RelationID, AttributeNumber = AttributeNumber });
            return result;
        }
    }

    internal class SublinkExpressionResult : ExpressionResult
    {
        public QueryTreeDataInternal SubQuery { get; set; }
        public SublinkExpressionResult(QueryTreeDataInternal source) : base(source)
        {

        }
        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            return new HashSet<QueryTreeAttribute>();
        }
        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            return new HashSet<QueryTreeRelation>();
        }
    }

    internal class ConstExpressionResult : ExpressionResult
    {
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }
        public ConstExpressionResult(QueryTreeDataInternal source) : base(source)
        {

        }
        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            return new HashSet<QueryTreeAttribute>();
        }
        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            return new HashSet<QueryTreeRelation>();
        }
    }
    internal class BooleanExpressionResult : ExpressionResult
    {
        public string Operator { get; set; }
        public List<ExpressionResult> Arguments { get; private set; }
        public BooleanExpressionResult(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<ExpressionResult>();
        }
        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            var result = new HashSet<QueryTreeRelation>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllRelations());
            }
            return result;
        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            var result = new HashSet<QueryTreeAttribute>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllAttributes());
            }
            return result;
        }
    }
    internal class OperatorExpressionResult : ExpressionResult
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public uint OperatorID { get; set; }
        public List<ExpressionResult> Arguments { get; private set; }
        public OperatorExpressionResult(QueryTreeDataInternal source) : base(source)
        {
            Arguments = new List<ExpressionResult>();
        }

        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            var result = new HashSet<QueryTreeRelation>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllRelations());
            }
            return result;
        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            var result = new HashSet<QueryTreeAttribute>();
            foreach (var a in Arguments)
            {
                result.AddRange(a.GetAllAttributes());
            }
            return result;
        }
    }

    internal class NullTestExpressionResult : ExpressionResult
    {
        public NullTestType TestType { get; set; }
        public ExpressionResult Argument { get; set; }
        public NullTestExpressionResult(QueryTreeDataInternal source) : base(source)
        {
            TestType = NullTestType.Unknown;
        }
        public override ISet<QueryTreeRelation> GetAllRelations()
        {
            var result = new HashSet<QueryTreeRelation>();
            result.AddRange(Argument.GetAllRelations());
            return result;
        }

        public override ISet<QueryTreeAttribute> GetAllAttributes()
        {
            var result = new HashSet<QueryTreeAttribute>();
            result.AddRange(Argument.GetAllAttributes());
            return result;
        }
    }
    internal class Rte
    {
        public RteKind RteKind { get; set; }

        // Relation
        public RelKind RelKind { get; set; }
        public uint? RelId { get; set; }

        // Subquery
        public QueryTreeDataInternal SubQuery { get; set; }

        // Join
        public JoinType JoinType { get; set; }
        public List<EvalExpression> JoinVars { get; private set; }
        // CTE
        public string CteName { get; set; }
        public Rte()
        {
            JoinType = JoinType.Unknown;
            JoinVars = new List<EvalExpression>();
        }
    }
    internal class QueryTreeDataInternal
    {
        public CmdType CommandType { get; set; }
        public QueryTreeDataInternal Parent { get; set; }
        public Dictionary<string, QueryTreeDataInternal> Ctes { get; private set; }
        public List<Rte> Rtes { get; private set; }
        public List<EvalExpression> TargetEntries { get; private set; }
        public List<EvalExpression> JoinQuals { get; private set; }
        public List<EvalExpression> WhereQuals { get; private set; }
        public List<EvalExpression> HavingQuals { get; private set; }
        public List<EvalExpression> OrderByEntries { get; private set; }
        public List<EvalExpression> GroupByEntries { get; private set; }
        public QueryTreeDataInternal()
        {
            Ctes = new Dictionary<string, QueryTreeDataInternal>();
            Rtes = new List<Rte>();
            TargetEntries = new List<EvalExpression>();
            JoinQuals = new List<EvalExpression>();
            WhereQuals = new List<EvalExpression>();
            HavingQuals = new List<EvalExpression>();
            OrderByEntries = new List<EvalExpression>();
            GroupByEntries = new List<EvalExpression>();
        }
    }
}
