using IndexSuggestions.Common;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.IndexAnalysis
{
    internal class ExtractStatementsQueryDataCommand : ChainableCommand
    {
        private static readonly HashSet<string> supportedOperators = new HashSet<string>(new string[] { "<", "<=", "=", ">=", ">" });
        private readonly DesignIndicesContext context;
        private readonly DBMS.Contracts.IRelationsRepository relationsRepository;
        private readonly DBMS.Contracts.IRelationAttributesRepository attributesRepository;
        private readonly DBMS.Contracts.IExpressionOperatorsRepository expressionsRepository;
        public ExtractStatementsQueryDataCommand(DesignIndicesContext context, DBMS.Contracts.IRepositoriesFactory dbmsRepositories)
        {
            this.context = context;
            this.relationsRepository = dbmsRepositories.GetRelationsRepository();
            this.attributesRepository = dbmsRepositories.GetRelationAttributesRepository();
            this.expressionsRepository = dbmsRepositories.GetExpressionOperatorsRepository();
        }
        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var kv in context.Statements)
                {
                    var statement = kv.Value;
                    foreach (var query in statement.NormalizedStatement.StatementDefinition.IndependentQueries)
                    {
                        StatementQueryExtractedData data = new StatementQueryExtractedData();
                        data.WhereAttributes = GetAllAttributesFromExpressions(query.WhereExpressions);
                        data.JoinAttributes = GetAllAttributesFromExpressions(query.JoinExpressions);
                        data.GroupByAttributes = GetAllAttributesFromExpressions(query.GroupByExpressions);
                        data.OrderByAttributes = GetAllAttributesFromExpressions(query.OrderByExpressions);
                        data.ProjectionAttributes = new HashSet<IndexAttribute>();
                        foreach (var t in query.ProjectionAttributes)
                        {
                            var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                            if (attribute != null)
                            {
                                if (supportedOperators.Intersect(attribute.SupportedOperators).Count() > 0 // is comparable operator
                                                        && !context.Workload.Definition.Relations.ForbiddenValues.Contains(t.RelationID)) // relation is not forbidden
                                {
                                    var relation = relationsRepository.Get(t.RelationID);
                                    data.ProjectionAttributes.Add(CreateIndexAttribute(relation, attribute));
                                } 
                            }
                        }
                        context.IndicesDesignData.StatementsExtractedData.Add(statement.NormalizedStatement, query, data);
                    }
                }
            }
        }

        private HashSet<IndexAttribute> GetAllAttributesFromExpressions(IEnumerable<StatementQueryExpression> expressions)
        {
            HashSet<IndexAttribute> attributes = new HashSet<IndexAttribute>();
            foreach (var e in expressions)
            {
                if (e is StatementQueryAttributeExpression)
                {
                    var t = (StatementQueryAttributeExpression)e;
                    var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                    if (attribute != null)
                    {
                        if (supportedOperators.Intersect(attribute.SupportedOperators).Count() > 0 // is comparable operator
                                        && !context.Workload.Definition.Relations.ForbiddenValues.Contains(t.RelationID)) // relation is not forbidden
                        {
                            var relation = relationsRepository.Get(t.RelationID);
                            attributes.Add(CreateIndexAttribute(relation, attribute));
                        } 
                    }
                }
                else if (e is StatementQueryBooleanExpression)
                {
                    var t = (StatementQueryBooleanExpression)e;
                    attributes.AddRange(GetAllAttributesFromExpressions(t.Arguments));
                }
                else if (e is StatementQueryFunctionExpression)
                {
                    var t = (StatementQueryFunctionExpression)e;
                    attributes.AddRange(GetAllAttributesFromExpressions(t.Arguments));
                }
                else if (e is StatementQueryNullTestExpression)
                {
                    var t = (StatementQueryNullTestExpression)e;
                    attributes.AddRange(GetAllAttributesFromExpressions(new[] { t.Argument }));
                }
                else if (e is StatementQueryOperatorExpression)
                {
                    var t = (StatementQueryOperatorExpression)e;
                    var expressionOperator = expressionsRepository.Get(t.OperatorID);
                    if (expressionOperator != null && supportedOperators.Contains(expressionOperator.Name)) // is comparable operator
                    {
                        attributes.AddRange(GetAllAttributesFromExpressions(t.Arguments));
                    }
                }
            }
            return attributes;
        }

        private IndexAttribute CreateIndexAttribute(IRelation relation, IRelationAttribute attribute)
        {
            return new IndexAttribute(CreateIndexRelation(relation), attribute.Name, attribute.DbType, attribute.CardinalityIndicator, attribute.MostCommonValues, attribute.MostCommonValuesFrequencies);
        }

        private IndexRelation CreateIndexRelation(IRelation relation)
        {
            return new IndexRelation(relation.ID, relation.Name, relation.SchemaName, relation.DatabaseName);
        }
    }
}
