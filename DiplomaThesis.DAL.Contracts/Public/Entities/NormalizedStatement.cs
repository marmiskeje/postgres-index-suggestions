﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class NormalizedStatement : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [MaxLength(8000)]
        [Required]
        public string Statement { get; set; }
        [MaxLength(100)]
        [Required]
        public string StatementFingerprint { get; set; }
        public string StatementDefinitionData { get; set; }
        [NotMapped]
        public StatementDefinition StatementDefinition { get; set; }
        public StatementQueryCommandType? CommandType { get; set; }
        public List<NormalizedStatementStatistics> NormalizedStatementStatistics { get; set; }
        public List<NormalizedStatementIndexStatistics> NormalizedStatementIndexStatistics { get; set; }
        public List<NormalizedStatementRelationStatistics> NormalizedStatementRelationStatistics { get; set; }
        public List<VirtualEnvironmentStatementEvaluation> VirtualEnvironmentStatementEvaluations { get; set; }
        public List<WorkloadAnalysisRealStatementEvaluation> WorkloadAnalysisRealStatementEvaluations { get; set; }
        public List<VirtualEnvironmentPossibleCoveringIndex> VirtualEnvironmentPossibleCoveringIndices { get; set; }
    }

    public class StatementDefinition
    {
        public StatementQueryCommandType CommandType { get; set; }
        public List<StatementQuery> IndependentQueries { get; set; }
        public string Fingerprint { get; set; }
        public StatementDefinition()
        {
            IndependentQueries = new List<StatementQuery>();
        }
        
    }

    public class StatementQuery
    {
        public string Fingerprint { get; set; }
        public StatementQueryCommandType CommandType { get; set; }
        public ISet<StatementQueryRelation> Relations { get; set; }
        public ISet<StatementQueryAttribute> ProjectionAttributes { get; set; }
        public IList<StatementQueryExpression> WhereExpressions { get; set; }
        public IList<StatementQueryExpression> JoinExpressions { get; set; }
        public IList<StatementQueryExpression> HavingExpressions { get; set; }
        public IList<StatementQueryExpression> OrderByExpressions { get; set; }
        public IList<StatementQueryExpression> GroupByExpressions { get; set; }
        public StatementQuery()
        {
            Relations = new HashSet<StatementQueryRelation>();
            ProjectionAttributes = new HashSet<StatementQueryAttribute>();
            WhereExpressions = new List<StatementQueryExpression>();
            JoinExpressions = new List<StatementQueryExpression>();
            HavingExpressions = new List<StatementQueryExpression>();
            OrderByExpressions = new List<StatementQueryExpression>();
            GroupByExpressions = new List<StatementQueryExpression>();
        }
    }

    public enum StatementQueryCommandType
    {
        Unknown = 0,
        Select = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        Utility = 5
    }

    public class StatementQueryRelation
    {
        public uint ID { get; set; }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is StatementQueryRelation))
            {
                return false;
            }
            StatementQueryRelation tmp = (StatementQueryRelation)obj;
            return ID.Equals(tmp.ID);
        }

        public string CalculateFingerprint()
        {
            return ID.ToString();
        }
    }
    public class StatementQueryAttribute
    {
        public int AttributeNumber { get; set; }
        public uint RelationID { get; set; }
        public bool WithAppliedAggregateFunction { get; set; }

        public override int GetHashCode()
        {
            return $"{RelationID}_{AttributeNumber}_{WithAppliedAggregateFunction}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is StatementQueryAttribute))
            {
                return false;
            }
            StatementQueryAttribute tmp = (StatementQueryAttribute)obj;
            return RelationID.Equals(tmp.RelationID) && AttributeNumber.Equals(tmp.AttributeNumber) && WithAppliedAggregateFunction.Equals(tmp.WithAppliedAggregateFunction);
        }

        public string CalculateFingerprint()
        {
            return $"{RelationID}_{AttributeNumber}_{WithAppliedAggregateFunction}";
        }
    }

    public abstract class StatementQueryExpression
    {
        public abstract string CalculateFingerprint();
    }

    public class StatementQueryFunctionExpression : StatementQueryExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public List<StatementQueryExpression> Arguments { get; private set; }
        public StatementQueryFunctionExpression()
        {
            Arguments = new List<StatementQueryExpression>();
        }

        public override string CalculateFingerprint()
        {
            List<string> parts = new List<string>();
            parts.Add("Function");
            parts.Add(ResultTypeID.ToString());
            foreach (var item in Arguments)
            {
                parts.Add(item.CalculateFingerprint());
            }
            return string.Join("_", parts);
        }
    }

    public class StatementQueryUnknownExpression : StatementQueryExpression
    {
        public override string CalculateFingerprint()
        {
            return "UnknownExpression";
        }
    }

    public class StatementQueryAttributeExpression : StatementQueryExpression
    {
        public uint RelationID { get; set; }
        public int AttributeNumber { get; set; }
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }

        public override string CalculateFingerprint()
        {
            return $"{RelationID}_{AttributeNumber}_{TypeID}";
        }
    }

    public class StatementQueryConstExpression : StatementQueryExpression
    {
        public uint TypeID { get; set; }
        public DbType DbType { get; set; }

        public override string CalculateFingerprint()
        {
            return $"{TypeID}";
        }
    }
    public class StatementQueryBooleanExpression : StatementQueryExpression
    {
        public string Operator { get; set; }
        public List<StatementQueryExpression> Arguments { get; private set; }
        public StatementQueryBooleanExpression()
        {
            Arguments = new List<StatementQueryExpression>();
        }

        public override string CalculateFingerprint()
        {
            List<string> parts = new List<string>();
            parts.Add(Operator);
            foreach (var item in Arguments)
            {
                parts.Add(item.CalculateFingerprint());
            }
            return string.Join("_", parts);
        }
    }
    public class StatementQueryOperatorExpression : StatementQueryExpression
    {
        public uint ResultTypeID { get; set; }
        public DbType ResultDbType { get; set; }
        public uint OperatorID { get; set; }
        public List<StatementQueryExpression> Arguments { get; private set; }
        public StatementQueryOperatorExpression()
        {
            Arguments = new List<StatementQueryExpression>();
        }

        public override string CalculateFingerprint()
        {
            List<string> parts = new List<string>();
            parts.Add(ResultTypeID.ToString());
            parts.Add(OperatorID.ToString());
            foreach (var item in Arguments)
            {
                parts.Add(item.CalculateFingerprint());
            }
            return string.Join("_", parts);
        }
    }

    public class StatementQueryAggregateExpression : StatementQueryExpression
    {
        public List<StatementQueryExpression> Arguments { get; private set; }
        public StatementQueryAggregateExpression()
        {
            Arguments = new List<StatementQueryExpression>();
        }

        public override string CalculateFingerprint()
        {
            List<string> parts = new List<string>();
            parts.Add("Aggregate");
            foreach (var item in Arguments)
            {
                parts.Add(item.CalculateFingerprint());
            }
            return string.Join("_", parts);
        }
    }

    public class StatementQueryNullTestExpression : StatementQueryExpression
    {
        public StatementQueryNullTestType TestType { get; set; }
        public StatementQueryExpression Argument { get; set; }

        public override string CalculateFingerprint()
        {
            return TestType.ToString() + "_" + Argument.CalculateFingerprint();
        }
    }

    public enum StatementQueryNullTestType
    {
        Unkown = 0,
        IsNull = 1,
        IsNotNull = 2
    }
}
