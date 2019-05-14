using DiplomaThesis.Common;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.WorkloadAnalyzer
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
                        var allWhereOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var btreeWhereAttributes = new HashSet<AttributeData>();
                        var allJoinOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var btreeJoinAttributes = new HashSet<AttributeData>();
                        var allGroupByOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var btreeGroupByAttributes = new HashSet<AttributeData>();
                        var allOrderByOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var btreeOrderByAttributes = new HashSet<AttributeData>();
                        FillAllAttributesAndOperatorsFromExpressions(allWhereOperatorsByAttribute, btreeWhereAttributes, query.WhereExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(allJoinOperatorsByAttribute, btreeJoinAttributes, query.JoinExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(allGroupByOperatorsByAttribute, btreeGroupByAttributes, query.GroupByExpressions, new HashSet<string>());
                        FillAllAttributesAndOperatorsFromExpressions(allOrderByOperatorsByAttribute, btreeOrderByAttributes, query.OrderByExpressions, new HashSet<string>());
                        var allProjectionOperatorsByAttribute = new Dictionary<AttributeData, ISet<string>>();
                        var btreeProjectionAttributes = new HashSet<AttributeData>();
                        foreach (var t in query.ProjectionAttributes)
                        {
                            var attribute = attributesRepository.Get(t.RelationID, t.AttributeNumber);
                            if (attribute != null)
                            {
                                if (context.RelationsData.TryGetRelation(t.RelationID, out var relationData))
                                {
                                    var toAdd = CreateIndexAttribute(relationData, attribute);
                                    if (!allProjectionOperatorsByAttribute.ContainsKey(toAdd))
                                    {
                                        allProjectionOperatorsByAttribute.Add(toAdd, new HashSet<string>());
                                    }
                                    if (supportedOperators.Intersect(attribute.SupportedOperators).Count() > 0 // is comparable operator
                                        && !context.Workload.Definition.Relations.ForbiddenValues.Contains(t.RelationID)) // relation is not forbidden
                                    {
                                        btreeProjectionAttributes.Add(toAdd);
                                    }
                                }
                            }
                        }
                        var data = new StatementQueryExtractedData(new SetOfAttributes(allWhereOperatorsByAttribute, btreeWhereAttributes),
                                                                   new SetOfAttributes(allJoinOperatorsByAttribute, btreeJoinAttributes),
                                                                   new SetOfAttributes(allGroupByOperatorsByAttribute, btreeGroupByAttributes),
                                                                   new SetOfAttributes(allOrderByOperatorsByAttribute, btreeOrderByAttributes),
                                                                   new SetOfAttributes(allProjectionOperatorsByAttribute, btreeProjectionAttributes));
                        context.StatementsExtractedData.Add(statement.NormalizedStatement, query, data);
                    }
                }
            }
        }

        private void FillAllAttributesAndOperatorsFromExpressions(Dictionary<AttributeData, ISet<string>> allOperatorsByAttribute,
                                                                  HashSet<AttributeData> bTreeApplicableAttributes,
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
                        if (context.RelationsData.TryGetRelation(t.RelationID, out var relationData))
                        {
                            var toAdd = CreateIndexAttribute(relationData, attribute);
                            if (!allOperatorsByAttribute.ContainsKey(toAdd))
                            {
                                allOperatorsByAttribute.Add(toAdd, new HashSet<string>());
                            }
                            allOperatorsByAttribute[toAdd].AddRange(operators);
                            if (supportedOperators.Intersect(attribute.SupportedOperators).Count() > 0 // is comparable operator
                                && !context.Workload.Definition.Relations.ForbiddenValues.Contains(t.RelationID) // relation is not forbidden
                                && operators.Except(supportedOperators).Count() == 0) // was applied comparable operator
                            {
                                bTreeApplicableAttributes.Add(toAdd);
                            }
                        }
                    }
                }
                else if (e is StatementQueryBooleanExpression)
                {
                    var t = (StatementQueryBooleanExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, bTreeApplicableAttributes, t.Arguments, operators); // ignore boolean operators
                }
                else if (e is StatementQueryFunctionExpression)
                {
                    var t = (StatementQueryFunctionExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, bTreeApplicableAttributes, t.Arguments, operators);
                }
                else if (e is StatementQueryNullTestExpression)
                {
                    var t = (StatementQueryNullTestExpression)e;
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, bTreeApplicableAttributes, new[] { t.Argument }, operators);
                }
                else if (e is StatementQueryOperatorExpression)
                {
                    var t = (StatementQueryOperatorExpression)e;
                    var expressionOperator = expressionsRepository.Get(t.OperatorID);
                    FillAllAttributesAndOperatorsFromExpressions(allOperatorsByAttribute, bTreeApplicableAttributes, t.Arguments, operators.Union(new[] { expressionOperator.Name }));
                }
            }
        }


        private AttributeData CreateIndexAttribute(RelationData relationData, IRelationAttribute attribute)
        {
            return new AttributeData(relationData, attribute);
        }
    }
}
