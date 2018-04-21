using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class SaveStatementDefinitionIfNotExistsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public SaveStatementDefinitionIfNotExistsCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            if (context.PersistedData.NormalizedStatement.StatementDefinition == null)
            {
                StatementDefinition definition = new StatementDefinition();
                definition.CommandType = Convert(context.QueryTree.CommandType);
                foreach (var query in context.QueryTree.IndependentQueries)
                {
                    var toAdd = new StatementQuery();
                    toAdd.CommandType = Convert(query.CommandType);
                    foreach (var expr in query.GroupByExpressions)
                    {
                        toAdd.GroupByExpressions.Add(Convert(expr));
                    }
                    foreach (var expr in query.HavingExpressions)
                    {
                        toAdd.HavingExpressions.Add(Convert(expr));
                    }
                    foreach (var expr in query.JoinExpressions)
                    {
                        toAdd.JoinExpressions.Add(Convert(expr));
                    }
                    foreach (var expr in query.OrderByExpressions)
                    {
                        toAdd.OrderByExpressions.Add(Convert(expr));
                    }
                    foreach (var a in query.ProjectionAttributes)
                    {
                        toAdd.ProjectionAttributes.Add(Convert(a));
                    }
                    foreach (var r in query.Relations)
                    {
                        toAdd.Relations.Add(Convert(r));
                    }
                    foreach (var expr in query.WhereExpressions)
                    {
                        toAdd.WhereExpressions.Add(Convert(expr));
                    }
                    definition.IndependentQueries.Add(toAdd);
                }
                context.PersistedData.NormalizedStatement.CommandType = definition.CommandType;
                context.PersistedData.NormalizedStatement.StatementDefinition = definition;
                var statements = repositories.GetNormalizedStatementsRepository();
                statements.Update(context.PersistedData.NormalizedStatement);
            }
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
