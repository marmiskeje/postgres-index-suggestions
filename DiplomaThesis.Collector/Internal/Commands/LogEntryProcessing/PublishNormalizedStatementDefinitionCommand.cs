using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class PublishNormalizedStatementDefinitionCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        private static readonly Dictionary<StatementQueryCommandType, int> commandTypeImportance = new Dictionary<StatementQueryCommandType, int>();
        public PublishNormalizedStatementDefinitionCommand(LogEntryProcessingContext context, IStatementsProcessingDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        static PublishNormalizedStatementDefinitionCommand()
        {
            commandTypeImportance.Add(StatementQueryCommandType.Unknown, 0);
            commandTypeImportance.Add(StatementQueryCommandType.Utility, 1);
            commandTypeImportance.Add(StatementQueryCommandType.Select, 2);
            commandTypeImportance.Add(StatementQueryCommandType.Delete, 3);
            commandTypeImportance.Add(StatementQueryCommandType.Update, 4);
            commandTypeImportance.Add(StatementQueryCommandType.Insert, 5);
        }
        protected override void OnExecute()
        {
            if (context.QueryTrees.Count > 0)
            {
                StatementDefinition definition = new StatementDefinition();
                
                Dictionary<string, StatementQuery> independentQueries = new Dictionary<string, StatementQuery>();
                foreach (var tree in context.QueryTrees)
                {
                    var commandType = Convert(tree.CommandType);
                    if (commandTypeImportance[commandType] > commandTypeImportance[definition.CommandType])
                    {
                        definition.CommandType = commandType;
                    }
                    foreach (var query in tree.IndependentQueries)
                    {
                        var toAdd = new StatementQuery();
                        toAdd.CommandType = Convert(query.CommandType);
                        StringBuilder fingerprintBuilder = new StringBuilder();
                        fingerprintBuilder.Append("_" + toAdd.CommandType.ToString());
                        foreach (var expr in query.GroupByExpressions)
                        {
                            var e = Convert(expr);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.GroupByExpressions.Add(e);
                        }
                        foreach (var expr in query.HavingExpressions)
                        {
                            var e = Convert(expr);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.HavingExpressions.Add(e);
                        }
                        foreach (var expr in query.JoinExpressions)
                        {
                            var e = Convert(expr);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.JoinExpressions.Add(e);
                        }
                        foreach (var expr in query.OrderByExpressions)
                        {
                            var e = Convert(expr);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.OrderByExpressions.Add(e);
                        }
                        foreach (var a in query.ProjectionAttributes)
                        {
                            var e = Convert(a);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.ProjectionAttributes.Add(e);
                        }
                        foreach (var r in query.Relations)
                        {
                            var e = Convert(r);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.Relations.Add(e);
                        }
                        foreach (var expr in query.WhereExpressions)
                        {
                            var e = Convert(expr);
                            fingerprintBuilder.Append("_" + e.CalculateFingerprint());
                            toAdd.WhereExpressions.Add(e);
                        }
                        using (var sha = SHA1.Create())
                        {
                            toAdd.Fingerprint = System.Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(fingerprintBuilder.ToString())));
                        }
                        if (!independentQueries.ContainsKey(toAdd.Fingerprint))
                        {
                            independentQueries.Add(toAdd.Fingerprint, toAdd);
                        }
                    }
                }
                definition.IndependentQueries.AddRange(independentQueries.Values);
                definition.Fingerprint = CalculateFingerprint(definition);
                statementDataAccumulator.PublishNormalizedStatementDefinition(context.StatementData.NormalizedStatementFingerprint, definition);
            }
            else
            {
                StatementDefinition definition = new StatementDefinition();
                definition.CommandType = context.StatementData.CommandType;
                statementDataAccumulator.PublishNormalizedStatementDefinition(context.StatementData.NormalizedStatementFingerprint, definition);
            }
        }

        private string CalculateFingerprint(StatementDefinition definition)
        {
            return $"{definition.CommandType}_{String.Join("_", definition.IndependentQueries.Select(x => x.Fingerprint))}";
        }

        private StatementQueryRelation Convert(QueryTreeRelation source)
        {
            StatementQueryRelation result = new StatementQueryRelation();
            result.ID = source.ID;
            return result;
        }

        private StatementQueryAttribute Convert(QueryTreeAttribute source)
        {
            StatementQueryAttribute result = new StatementQueryAttribute();
            result.AttributeNumber = source.AttributeNumber;
            result.RelationID = source.RelationID;
            result.WithAppliedAggregateFunction = source.WithAppliedAggregationFunction;
            return result;
        }

        private StatementQueryExpression Convert(QueryTreeExpression source)
        {
            if (source is QueryTreeFunctionExpression)
            {
                return ConvertFuncExpression(source as QueryTreeFunctionExpression);
            }
            else if (source is QueryTreeNullTestExpression)
            {
                return ConvertNullTestExpression(source as QueryTreeNullTestExpression);
            }
            else if (source is QueryTreeOperatorExpression)
            {
                return ConvertOperatorExpression(source as QueryTreeOperatorExpression);
            }
            else if (source is QueryTreeConstExpression)
            {
                return ConvertConstExpression(source as QueryTreeConstExpression);
            }
            else if (source is QueryTreeBooleanExpression)
            {
                return ConvertBooleanExpression(source as QueryTreeBooleanExpression);
            }
            else if (source is QueryTreeAttributeExpression)
            {
                return ConvertAttributeExpression(source as QueryTreeAttributeExpression);
            }
            else if (source is QueryTreeAggregateExpression)
            {
                return ConvertAggregateExpression(source as QueryTreeAggregateExpression);
            }
            return new StatementQueryUnknownExpression();
        }
        private StatementQueryFunctionExpression ConvertFuncExpression(QueryTreeFunctionExpression source)
        {
            StatementQueryFunctionExpression result = new StatementQueryFunctionExpression();
            foreach (var a in source.Arguments)
            {
                result.Arguments.Add(Convert(a));
            }
            result.ResultDbType = source.ResultDbType;
            result.ResultTypeID = source.ResultTypeID;
            return result;
        }
        private StatementQueryNullTestExpression ConvertNullTestExpression(QueryTreeNullTestExpression source)
        {
            StatementQueryNullTestExpression result = new StatementQueryNullTestExpression();
            result.Argument = Convert(source.Argument);
            result.TestType = Convert(source.TestType);
            return result;
        }

        private StatementQueryNullTestType Convert(QueryTreeNullTestType source)
        {
            switch (source)
            {
                case QueryTreeNullTestType.IsNull:
                    return StatementQueryNullTestType.IsNull;
                case QueryTreeNullTestType.IsNotNull:
                    return StatementQueryNullTestType.IsNotNull;
            }
            return StatementQueryNullTestType.Unkown;
        }

        private StatementQueryOperatorExpression ConvertOperatorExpression(QueryTreeOperatorExpression source)
        {
            StatementQueryOperatorExpression result = new StatementQueryOperatorExpression();
            foreach (var a in source.Arguments)
            {
                result.Arguments.Add(Convert(a));
            }
            result.OperatorID = source.OperatorID;
            result.ResultDbType = source.ResultDbType;
            result.ResultTypeID = source.ResultTypeID;
            return result;
        }
        private StatementQueryAggregateExpression ConvertAggregateExpression(QueryTreeAggregateExpression source)
        {
            StatementQueryAggregateExpression result = new StatementQueryAggregateExpression();
            foreach (var a in source.Arguments)
            {
                result.Arguments.Add(Convert(a));
            }
            return result;
        }
        private StatementQueryConstExpression ConvertConstExpression(QueryTreeConstExpression source)
        {
            StatementQueryConstExpression result = new StatementQueryConstExpression();
            result.DbType = source.DbType;
            result.TypeID = source.TypeID;
            return result;
        }
        private StatementQueryBooleanExpression ConvertBooleanExpression(QueryTreeBooleanExpression source)
        {
            StatementQueryBooleanExpression result = new StatementQueryBooleanExpression();
            foreach (var a in source.Arguments)
            {
                result.Arguments.Add(Convert(a));
            }
            result.Operator = source.Operator;
            return result;
        }
        private StatementQueryAttributeExpression ConvertAttributeExpression(QueryTreeAttributeExpression source)
        {
            StatementQueryAttributeExpression result = new StatementQueryAttributeExpression();
            result.AttributeNumber = source.AttributeNumber;
            result.DbType = source.DbType;
            result.RelationID = source.RelationID;
            result.TypeID = source.TypeID;
            return result;
        }

        private StatementQueryCommandType Convert(QueryCommandType source)
        {
            switch (source)
            {
                case QueryCommandType.Select:
                    return StatementQueryCommandType.Select;
                case QueryCommandType.Insert:
                    return StatementQueryCommandType.Insert;
                case QueryCommandType.Update:
                    return StatementQueryCommandType.Update;
                case QueryCommandType.Delete:
                    return StatementQueryCommandType.Delete;
                case QueryCommandType.Utility:
                    return StatementQueryCommandType.Utility;
            }
            return StatementQueryCommandType.Unknown;
        }
    }
}
