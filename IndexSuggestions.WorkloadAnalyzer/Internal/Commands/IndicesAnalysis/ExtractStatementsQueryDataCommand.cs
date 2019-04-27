using IndexSuggestions.Common;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class ExtractStatementsQueryDataCommand : ChainableCommand
    {
        private static readonly HashSet<string> supportedOperators = new HashSet<string>(new string[] { "<", "<=", "=", ">=", ">" });
        private readonly WorkloadAnalysisContext context;
        private readonly DBMS.Contracts.IRelationAttributesRepository attributesRepository;
        private readonly DBMS.Contracts.IExpressionOperatorsRepository expressionsRepository;
        public ExtractStatementsQueryDataCommand(WorkloadAnalysisContext context, DBMS.Contracts.IRepositoriesFactory dbmsRepositories)
        {
            this.context = context;
            this.attributesRepository = dbmsRepositories.GetRelationAttributesRepository();
            this.expressionsRepository = dbmsRepositories.GetExpressionOperatorsRepository();
        }
        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var kv in context.StatementsData.AllSelects)
                {
                    var statement = kv.Value;
                    foreach (var query in statement.NormalizedStatement.StatementDefinition.IndependentQueries)
                    {
                        var whereOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var joinOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var groupByOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var orderByOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        FillAllAttributesAndOperatorsFromExpressions(whereOperatorsByAttribute, query.WhereExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(joinOperatorsByAttribute, query.JoinExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(groupByOperatorsByAttribute, query.GroupByExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(orderByOperatorsByAttribute, query.OrderByExpressions, new HashSet<string>());
                        HashSet<AttributeData> projectionAttributes = new HashSet<AttributeData>();
                        foreach (var t in query.ProjectionAttributes)
                        {
                            var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                            if (attribute != null)
                            {
                                if (supportedOperators.Intersect(attribute.SupportedOperators).Count() > 0 // is comparable operator
                                                        && !context.Workload.Definition.Relations.ForbiddenValues.Contains(t.RelationID)) // relation is not forbidden
                                {
                                    if (context.RelationsData.TryGetRelation(t.RelationID, out var relationData))
                                    {
                                        projectionAttributes.Add(CreateIndexAttribute(relationData, attribute));
                                    }
                                } 
                            }
                        }
                        var data = new StatementQueryExtractedData(whereOperatorsByAttribute, joinOperatorsByAttribute, groupByOperatorsByAttribute, orderByOperatorsByAttribute, projectionAttributes);
                        context.StatementsExtractedData.Add(statement.NormalizedStatement, query, data);
                    }
                }
            }
        }

        private void FillAllAttributesAndOperatorsFromExpressions(Dictionary<AttributeData, ISet<string>> allOperatorsByAttribute,
                                                           IEnumerable<StatementQueryExpression> expressions, IEnumerable<string> operators)
        {
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
                            if (context.RelationsData.TryGetRelation(t.RelationID, out var relationData))
                            {
                                var toAdd = CreateIndexAttribute(relationData, attribute);
                                if (!allOperatorsByAttribute.ContainsKey(toAdd))
                                {
                                    allOperatorsByAttribute.Add(toAdd, new HashSet<string>());
                                }
                                allOperatorsByAttribute[toAdd].AddRange(operators);
                            }
                        }
                    }
                }
                else if (e is StatementQueryBooleanExpression)
                {
                    var t = (StatementQueryBooleanExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, t.Arguments, operators); // ignore boolean operators
                }
                else if (e is StatementQueryFunctionExpression)
                {
                    var t = (StatementQueryFunctionExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, t.Arguments, operators);
                }
                else if (e is StatementQueryNullTestExpression)
                {
                    var t = (StatementQueryNullTestExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, new[] { t.Argument }, operators);
                }
                else if (e is StatementQueryOperatorExpression)
                {
                    var t = (StatementQueryOperatorExpression)e;
                    var expressionOperator = expressionsRepository.Get(t.OperatorID);
                    if (expressionOperator != null && supportedOperators.Contains(expressionOperator.Name)) // is comparable operator
                    {
                        FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, t.Arguments, operators.Union(new[] { expressionOperator.Name }));
                    }
                }
            }
        }


        private AttributeData CreateIndexAttribute(RelationData relationData, IRelationAttribute attribute)
        {
            return new AttributeData(relationData, attribute);
        }
    }
}
