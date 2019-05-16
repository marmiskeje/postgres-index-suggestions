using DiplomaThesis.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;
using DiplomaThesis.DBMS.Contracts;
using DiplomaThesis.Common;
using DiplomaThesis.DBMS.Postgres;

namespace DiplomaThesis.Collector.Postgres
{
    internal class QueryTreeProvider
    {
        #region private QueryCommandType Convert(CmdType source)
        private QueryCommandType Convert(CmdType source)
        {
            switch (source)
            {
                case CmdType.Select:
                    return QueryCommandType.Select;
                case CmdType.Update:
                    return QueryCommandType.Update;
                case CmdType.Insert:
                    return QueryCommandType.Insert;
                case CmdType.Delete:
                    return QueryCommandType.Delete;
                case CmdType.Utility:
                    return QueryCommandType.Utility;
                default:
                    return QueryCommandType.Unknown;
            }
        }
        #endregion

        #region private QueryTreeExpression Convert(ExpressionResult source)
        private QueryTreeExpression Convert(ExpressionResult source)
        {
            if (source is ConstExpressionResult)
            {
                var r = (ConstExpressionResult)source;
                return new QueryTreeConstExpression() { DbType = r.DbType, TypeID = r.TypeID };
            }
            else if (source is AttributeExpressionResult)
            {
                var r = (AttributeExpressionResult)source;
                return new QueryTreeAttributeExpression() { DbType = r.DbType, TypeID = r.TypeID, AttributeNumber = r.AttributeNumber, RelationID = r.RelationID };
            }
            else if (source is FunctionExpressionResult)
            {
                var r = (FunctionExpressionResult)source;
                var result = new QueryTreeFunctionExpression() { ResultDbType = r.ResultDbType, ResultTypeID = r.ResultTypeID };
                foreach (var a in r.Arguments)
                {
                    result.Arguments.Add(Convert(a));
                }
                return result;
            }
            else if (source is OperatorExpressionResult)
            {
                var r = (OperatorExpressionResult)source;
                var result = new QueryTreeOperatorExpression() { OperatorID = r.OperatorID, ResultDbType = r.ResultDbType, ResultTypeID = r.ResultTypeID };
                foreach (var a in r.Arguments)
                {
                    result.Arguments.Add(Convert(a));
                }
                return result;
            }
            else if (source is AggregateExpressionResult)
            {
                var r = (AggregateExpressionResult)source;
                var result = new QueryTreeAggregateExpression();
                foreach (var a in r.Arguments)
                {
                    result.Arguments.Add(Convert(a));
                }
                return result;
            }
            else if (source is BooleanExpressionResult)
            {
                var r = (BooleanExpressionResult)source;
                var result = new QueryTreeBooleanExpression() { Operator = r.Operator };
                foreach (var a in r.Arguments)
                {
                    result.Arguments.Add(Convert(a));
                }
                return result;
            }
            else if (source is NullTestExpressionResult)
            {
                var r = (NullTestExpressionResult)source;
                return new QueryTreeNullTestExpression() { Argument = Convert(r.Argument), TestType = Convert(r.TestType) };
            }
            return new QueryTreeUnknownExpression();
        }

        private QueryTreeNullTestType Convert(NullTestType source)
        {
            switch (source)
            {
                case NullTestType.IsNull:
                    return QueryTreeNullTestType.IsNull;
                case NullTestType.IsNotNull:
                    return QueryTreeNullTestType.IsNotNull;
            }
            return QueryTreeNullTestType.Unkown;
        }
        #endregion

        public QueryTreeData Provide(JObject jObject)
        {
            var internalResult = ProcessQuery(null, jObject);
            var groupedRtesByQuery = new Dictionary<QueryTreeDataInternal, List<Rte>>();
            var groupedTargetResultsByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            var groupedWhereQualsByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            var groupedJoinQualsByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            var groupedHavingQualsByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            var groupedGroupBysByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            var groupedOrderBysByQuery = new Dictionary<QueryTreeDataInternal, List<ExpressionResult>>();
            FillAllRtesGroups(internalResult, groupedRtesByQuery, groupedTargetResultsByQuery, groupedWhereQualsByQuery, groupedJoinQualsByQuery, groupedHavingQualsByQuery, groupedGroupBysByQuery, groupedOrderBysByQuery);

            QueryTreeData result = new QueryTreeData();
            result.CommandType = Convert(internalResult.CommandType);
            var queryIndexMap = new Dictionary<QueryTreeDataInternal, int>();
            int index = 0;
            foreach (var query in groupedTargetResultsByQuery.Keys)
            {
                queryIndexMap.Add(query, index);
                result.IndependentQueries.Add(new QueryData()
                {
                    CommandType = Convert(query.CommandType)
                });
                index++;
            }
            foreach (var g in groupedRtesByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    if (item.RteKind == RteKind.Relation && item.RelKind == RelKind.Relation && item.RelId.HasValue)
                    {
                        result.IndependentQueries[queryIndex].Relations.Add(new QueryTreeRelation() { ID = item.RelId.Value });
                    }
                }
            }
            foreach (var g in groupedTargetResultsByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].ProjectionAttributes.AddRange(item.GetAllAttributes());
                }
            }
            foreach (var g in groupedWhereQualsByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].WhereExpressions.Add(Convert(item));
                }
            }
            foreach (var g in groupedJoinQualsByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].JoinExpressions.Add(Convert(item));
                }
            }
            foreach (var g in groupedHavingQualsByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].HavingExpressions.Add(Convert(item));
                }
            }
            foreach (var g in groupedGroupBysByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].GroupByExpressions.Add(Convert(item));
                }
            }
            foreach (var g in groupedOrderBysByQuery)
            {
                var queryIndex = queryIndexMap[g.Key];
                foreach (var item in g.Value)
                {
                    result.IndependentQueries[queryIndex].Relations.AddRange(item.GetAllRelations());
                    result.IndependentQueries[queryIndex].OrderByExpressions.Add(Convert(item));
                }
            }
            return result;
        }

        private void FillAllRtesGroups(QueryTreeDataInternal node, Dictionary<QueryTreeDataInternal, List<Rte>> rtes,
                                       Dictionary<QueryTreeDataInternal, List<ExpressionResult>> targetResults, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> whereQuals,
                                       Dictionary<QueryTreeDataInternal, List<ExpressionResult>> joinQuals, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> havingQuals,
                                       Dictionary<QueryTreeDataInternal, List<ExpressionResult>> groupBys, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> orderBys)
        {
            if (node == null)
            {
                return;
            }
            foreach (var rte in node.Rtes)
            {
                var query = node;
                if (rte.SubQuery != null)
                {
                    query = rte.SubQuery;
                    FillAllRtesGroups(query, rtes, targetResults, whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                }
                else
                {
                    if (!targetResults.ContainsKey(query))
                    {
                        targetResults.Add(query, new List<ExpressionResult>());
                        foreach (var e in query.TargetEntries)
                        {
                            var targetResult = ResolveExpression(e);
                            targetResults[query].Add(targetResult);
                            if (targetResult is SublinkExpressionResult)
                            {
                                var sublink = (SublinkExpressionResult)targetResult;
                                FillAllRtesGroups(sublink.SubQuery, rtes, targetResults, whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                            }
                        }
                    }
                    if (!whereQuals.ContainsKey(query))
                    {
                        whereQuals.Add(query, new List<ExpressionResult>());
                        foreach (var q in query.WhereQuals)
                        {
                            var exprResult = ResolveExpression(q);
                            whereQuals[query].Add(exprResult);
                            if (exprResult is SublinkExpressionResult)
                            {
                                var sublink = (SublinkExpressionResult)exprResult;
                                FillAllRtesGroups(sublink.SubQuery, rtes, targetResults, whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                            }
                        }
                    }
                    if (!joinQuals.ContainsKey(query))
                    {
                        joinQuals.Add(query, new List<ExpressionResult>());
                        foreach (var q in query.JoinQuals)
                        {
                            var exprResult = ResolveExpression(q);
                            joinQuals[query].Add(exprResult);
                            if (exprResult is SublinkExpressionResult)
                            {
                                var sublink = (SublinkExpressionResult)exprResult;
                                FillAllRtesGroups(sublink.SubQuery, rtes, targetResults, whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                            }
                        }
                    }
                    if (!havingQuals.ContainsKey(query))
                    {
                        havingQuals.Add(query, new List<ExpressionResult>());
                        foreach (var q in query.HavingQuals)
                        {
                            var exprResult = ResolveExpression(q);
                            havingQuals[query].Add(exprResult);
                            if (exprResult is SublinkExpressionResult)
                            {
                                var sublink = (SublinkExpressionResult)exprResult;
                                FillAllRtesGroups(sublink.SubQuery, rtes, targetResults, whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                            }
                        }
                    }
                    if (!groupBys.ContainsKey(query))
                    {
                        groupBys.Add(query, new List<ExpressionResult>());
                        foreach (var q in query.GroupByEntries)
                        {
                            groupBys[query].Add(ResolveExpression(q));
                        }
                    }
                    if (!orderBys.ContainsKey(query))
                    {
                        orderBys.Add(query, new List<ExpressionResult>());
                        foreach (var q in query.OrderByEntries)
                        {
                            orderBys[query].Add(ResolveExpression(q));
                        }
                    }
                    if (rte.CteName != null)
                    {
                        FillFromCte(query, FindCte(query, rte.CteName), whereQuals, joinQuals, havingQuals, groupBys, orderBys);
                    }
                }
                if (!rtes.ContainsKey(query))
                {
                    rtes.Add(query, new List<Rte>());
                }
                rtes[query].Add(rte);
            }
        }

        private void FillFromCte(QueryTreeDataInternal query, QueryTreeDataInternal cte, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> whereQuals,
                                 Dictionary<QueryTreeDataInternal, List<ExpressionResult>> joinQuals, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> havingQuals,
                                 Dictionary<QueryTreeDataInternal, List<ExpressionResult>> groupBys, Dictionary<QueryTreeDataInternal, List<ExpressionResult>> orderBys)
        {
            if (!whereQuals.ContainsKey(query))
            {
                whereQuals.Add(query, new List<ExpressionResult>());
            }
            if (!joinQuals.ContainsKey(query))
            {
                joinQuals.Add(query, new List<ExpressionResult>());
            }
            if (!havingQuals.ContainsKey(query))
            {
                havingQuals.Add(query, new List<ExpressionResult>());
            }
            if (!groupBys.ContainsKey(query))
            {
                groupBys.Add(query, new List<ExpressionResult>());
            }
            if (!orderBys.ContainsKey(query))
            {
                orderBys.Add(query, new List<ExpressionResult>());
            }
            foreach (var q in cte.WhereQuals)
            {
                whereQuals[query].Add(ResolveExpression(q));
            }
            foreach (var q in cte.JoinQuals)
            {
                joinQuals[query].Add(ResolveExpression(q));
            }
            foreach (var q in cte.HavingQuals)
            {
                havingQuals[query].Add(ResolveExpression(q));
            }
            foreach (var q in cte.GroupByEntries)
            {
                groupBys[query].Add(ResolveExpression(q));
            }
            foreach (var q in cte.OrderByEntries)
            {
                orderBys[query].Add(ResolveExpression(q));
            }
        }

        private QueryTreeDataInternal ProcessQuery(QueryTreeDataInternal parent, JToken query)
        {
            QueryTreeDataInternal result = new QueryTreeDataInternal();
            result.Parent = parent;
            result.CommandType = ParseCommandType(query);
            ProcessCtes(result, query);
            ProcessRtes(result, query);
            ProcessTargetEntries(result, query);
            ProcessJoinTree(result, query);
            ProcessHavingClause(result, query);
            ProcessGroupClause(result, query);
            ProcessSortClause(result, query);
            return result;
        }

        private CmdType ParseCommandType(JToken jObject)
        {
            var cmdType = jObject.SelectToken("QUERY.commandType");
            return EnumParsingSupport.ConvertFromNumericOrDefault<CmdType>(cmdType.Value<int>());
        }

        private void ProcessCtes(QueryTreeDataInternal result, JToken query)
        {
            foreach (var cteChild in query.SelectToken("QUERY.cteList").Children())
            {
                var cteName = cteChild.SelectToken("COMMONTABLEEXPR.ctename").Value<string>();
                result.Ctes.Add(cteName, ProcessQuery(result, cteChild.SelectToken("COMMONTABLEEXPR.ctequery")));
            }
        }

        private void ProcessRtes(QueryTreeDataInternal result, JToken query)
        {
            var rTable = query.SelectToken("QUERY.rtable");
            foreach (var child in rTable.Children())
            {
                var rte = child.SelectToken("RTE");
                Rte rteToAdd = new Rte();
                rteToAdd.RteKind = EnumParsingSupport.ConvertFromNumericOrDefault<RteKind>(rte.SelectToken("rtekind").Value<int>());
                switch (rteToAdd.RteKind)
                {
                    case RteKind.Relation:
                        {
                            rteToAdd.RelId = rte.SelectToken("relid").Value<uint>();
                            rteToAdd.RelKind = EnumParsingSupport.ConvertUsingAttributeOrDefault<RelKind, EnumMemberAttribute, string>(rte.SelectToken("relkind").Value<string>(), x => x.Value);
                        }
                        break;
                    case RteKind.Subquery:
                        {
                            rteToAdd.SubQuery = ProcessQuery(result, rte.SelectToken("subquery"));
                        }
                        break;
                    case RteKind.Join:
                        {
                            rteToAdd.JoinType = EnumParsingSupport.ConvertFromNumericOrDefault<JoinType>(rte.SelectToken("jointype").Value<int>());
                            foreach (var expr in rte.SelectToken("joinaliasvars").Children())
                            {
                                rteToAdd.JoinVars.Add(ResolveExpression(result, expr));
                            }
                        }
                        break;
                    case RteKind.Function:
                        break;
                    case RteKind.TableFunc:
                        break;
                    case RteKind.Values:
                        break;
                    case RteKind.CTE:
                        {
                            rteToAdd.CteName = rte.SelectToken("ctename").Value<string>();
                        }
                        break;
                    case RteKind.NamedTupleStore:
                        break;
                }
                result.Rtes.Add(rteToAdd);
            }
        }

        private ExpressionResult ResolveExpression(EvalExpression expr)
        {
            if (expr is VariableExpression)
            {
                return ResolveVariableExpression(expr as VariableExpression);
            }
            else if (expr is ConstExpression)
            {
                return ResolveConstExpression(expr as ConstExpression);
            }
            else if (expr is FunctionExpression)
            {
                return ResolveFunctionExpression(expr as FunctionExpression);
            }
            else if (expr is SublinkExpression)
            {
                return new SublinkExpressionResult(expr.Source) { SubQuery = ((SublinkExpression)expr).SubQuery };
            }
            else if (expr is BooleanExpression)
            {
                return ResolveBooleanExpression(expr as BooleanExpression);
            }
            else if (expr is OperatorExpression)
            {
                return ResolveOperatorExpression(expr as OperatorExpression);
            }
            else if (expr is NullTestExpression)
            {
                return ResolveNullTestExpression(expr as NullTestExpression);
            }
            else if (expr is AggregateExpression)
            {
                return ResolveAggregateExpression(expr as AggregateExpression);
            }
            return new UnknownExpressionResult(expr.Source);
        }
        private ExpressionResult ResolveConstExpression(ConstExpression expr)
        {
            return new ConstExpressionResult(expr.Source) { DbType = expr.DbType, TypeID = expr.TypeID };
        }
        private ExpressionResult ResolveNullTestExpression(NullTestExpression expr)
        {
            var result = new NullTestExpressionResult(expr.Source) { TestType = expr.TestType };
            result.Argument = ResolveExpression(expr.Argument);
            return result;
        }
        private ExpressionResult ResolveOperatorExpression(OperatorExpression expr)
        {
            var result = new OperatorExpressionResult(expr.Source) { OperatorID = expr.OperatorID, ResultDbType = expr.ResultDbType, ResultTypeID = expr.ResultTypeID };
            foreach (var a in expr.Arguments)
            {
                result.Arguments.Add(ResolveExpression(a));
            }
            return result;
        }
        private ExpressionResult ResolveAggregateExpression(AggregateExpression expr)
        {
            var result = new AggregateExpressionResult(expr.Source);
            foreach (var a in expr.Arguments)
            {
                result.Arguments.Add(ResolveExpression(a));
            }
            return result;
        }
        private ExpressionResult ResolveBooleanExpression(BooleanExpression expr)
        {
            var result = new BooleanExpressionResult(expr.Source) { Operator = expr.Operator };
            foreach (var a in expr.Arguments)
            {
                result.Arguments.Add(ResolveExpression(a));
            }
            return result;
        }
        private ExpressionResult ResolveFunctionExpression(FunctionExpression function)
        {
            var result = new FunctionExpressionResult(function.Source) { ResultDbType = function.ResultDbType, ResultTypeID = function.ResultTypeID };
            foreach (var a in function.Arguments)
            {
                result.Arguments.Add(ResolveExpression(a));
            }
            return result;
        }

        private ExpressionResult ResolveVariableExpression(VariableExpression variable)
        {
            var rte = variable.Source.Rtes[variable.RteNumber - 1];
            switch (rte.RteKind)
            {
                case RteKind.Relation:
                    {
                        return new AttributeExpressionResult(variable.Source) { AttributeNumber = variable.Position, RelationID = rte.RelId.Value, DbType = variable.DbType, TypeID = variable.TypeID };
                    }
                case RteKind.Subquery:
                    {
                        return ResolveExpression(rte.SubQuery.TargetEntries[variable.Position - 1]);
                        /*
                        return ResolveExpression(rte.SubQuery.TargetEntries.First(x =>
                        {
                            if (x is VariableExpression)
                            {
                                var a = (VariableExpression)x;
                                return variable.Position == a.Position;
                            }
                            return false;
                        }));*/
                    }
                case RteKind.Join:
                    {
                        return ResolveExpression(rte.JoinVars[variable.Position - 1]);
                        /*
                        return ResolveExpression(rte.JoinVars.First(x =>
                        {
                            if (x is VariableExpression)
                            {
                                var a = (VariableExpression)x;
                                return variable.Position == a.Position && variable.RteNumber == a.RteNumber;
                            }
                            return false;
                        }));*/
                    }
                case RteKind.Function:
                    break;
                case RteKind.TableFunc:
                    break;
                case RteKind.Values:
                    break;
                case RteKind.CTE:
                    {
                        var cte = FindCte(variable.Source, rte.CteName);
                        return ResolveExpression(cte.TargetEntries[variable.Position - 1]);
                        /*
                        return ResolveExpression(cte.TargetEntries.First(x =>
                        {
                            if (x is VariableExpression)
                            {
                                var a = (VariableExpression)x;
                                return variable.Position == a.Position && variable.RteNumber == a.RteNumber;
                            }
                            return false;
                        }));*/
                    }
                case RteKind.NamedTupleStore:
                    break;
            }
            return new UnknownExpressionResult(variable.Source);
        }

        private QueryTreeDataInternal FindCte(QueryTreeDataInternal data, string cteName)
        {
            if (data.Ctes.ContainsKey(cteName))
            {
                return data.Ctes[cteName];
            }
            return FindCte(data.Parent, cteName);
        }

        private EvalExpression ResolveExpression(QueryTreeDataInternal data, JToken expression, int? forcedRteNumber = null)
        {
            var exprType = expression.First.Value<JProperty>().Name;
            var expressionData = expression?.First?.First;
            switch (exprType)
            {
                case "CONST":
                    {
                        var typeId = expressionData.SelectToken("consttype").Value<uint>();
                        return new ConstExpression(data)
                        {
                            TypeID = typeId,
                            DbType = PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(typeId, x => x.OID))
                        };
                    }
                case "VAR":
                    {
                        var rteNumber = forcedRteNumber.HasValue ? forcedRteNumber.Value : expressionData.SelectToken("varnoold").Value<int>();
                        var attributeNumber = expressionData.SelectToken("varoattno").Value<int>();
                        var levelUpIndex = expressionData.SelectToken("varlevelsup").Value<int>();
                        var typeId = expressionData.SelectToken("vartype").Value<uint>();
                        var dataToUse = data;
                        if (levelUpIndex > 0)
                        {
                            for (int i = 0; i < levelUpIndex; i++)
                            {
                                dataToUse = dataToUse.Parent;
                            }
                        }
                        return new VariableExpression(dataToUse, rteNumber, attributeNumber)
                        {
                            TypeID = typeId,
                            DbType = PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(typeId, x => x.OID))
                        };
                    }
                case "RELABELTYPE":
                    {
                        var typeId = expressionData.SelectToken("resulttype").Value<uint>();
                        expressionData = expressionData.SelectToken("arg");
                        if (expressionData?.First?.Value<JProperty>().Name == "VAR")
                        {
                            expressionData = expressionData.First.First;
                            var rteNumber = forcedRteNumber.HasValue ? forcedRteNumber.Value : expressionData.SelectToken("varnoold").Value<int>();
                            var attributeNumber = expressionData.SelectToken("varoattno").Value<int>();
                            var levelUpIndex = expressionData.SelectToken("varlevelsup").Value<int>();
                            var dataToUse = data;
                            if (levelUpIndex > 0)
                            {
                                for (int i = 0; i < levelUpIndex; i++)
                                {
                                    dataToUse = dataToUse.Parent;
                                }
                            }
                            return new VariableExpression(dataToUse, rteNumber, attributeNumber)
                            {
                                TypeID = typeId,
                                DbType = PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(typeId, x => x.OID))
                            };
                        }
                        break;
                    }
                case "FUNCEXPR":
                    {
                        var typeId = expressionData.SelectToken("funcresulttype").Value<uint>();
                        var result = new FunctionExpression(data)
                        {
                            ResultTypeID = typeId,
                            ResultDbType = PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(typeId, x => x.OID))
                        };
                        foreach (var argExpr in expressionData.SelectToken("args").Children())
                        {
                            result.Arguments.Add(ResolveExpression(data, argExpr, forcedRteNumber));
                        }
                        return result;
                    }
                case "SUBLINK":
                    {
                        return new SublinkExpression(data) { SubQuery = ProcessQuery(data, expressionData.SelectToken("subselect")) };
                    }
                case "BOOLEXPR":
                    {
                        var result = new BooleanExpression(data);
                        result.Operator = expressionData.SelectToken("boolop").Value<string>();
                        foreach (var argExpr in expressionData.SelectToken("args").Children())
                        {
                            result.Arguments.Add(ResolveExpression(data, argExpr, forcedRteNumber));
                        }
                        return result;
                    }
                case "OPEXPR":
                    {
                        var typeId = expressionData.SelectToken("opresulttype").Value<uint>();
                        var result = new OperatorExpression(data)
                        {
                            ResultTypeID = typeId,
                            ResultDbType = PostgresDbTypeCovertUtility.Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(typeId, x => x.OID))
                        };
                        result.OperatorID = expressionData.SelectToken("opno").Value<uint>();
                        foreach (var argExpr in expressionData.SelectToken("args").Children())
                        {
                            result.Arguments.Add(ResolveExpression(data, argExpr, forcedRteNumber));
                        }
                        return result;
                    }
                case "AGGREF":
                    {
                        var result = new AggregateExpression(data);
                        foreach (var argExpr in expressionData.SelectToken("args").Children())
                        {
                            result.Arguments.Add(ResolveExpression(data, argExpr, forcedRteNumber));
                        }
                        return result;
                    }
                case "TARGETENTRY":
                    {
                        var expr = expressionData.SelectToken("expr");
                        return ResolveExpression(data, expr, forcedRteNumber);
                    }
                case "SCALARARRAYOPEXPR":
                    {
                        var result = new OperatorExpression(data);
                        result.OperatorID = expressionData.SelectToken("opno").Value<uint>();
                        foreach (var argExpr in expressionData.SelectToken("args").Children())
                        {
                            result.Arguments.Add(ResolveExpression(data, argExpr, forcedRteNumber));
                        }
                        return result;
                    }
                case "NULLTEST":
                    {
                        var result = new NullTestExpression(data);
                        result.TestType = EnumParsingSupport.ConvertFromNumericOrDefault<NullTestType>(expressionData.SelectToken("nulltesttype").Value<int>());
                        result.Argument = ResolveExpression(data, expressionData.SelectToken("arg"), forcedRteNumber);
                        return result;
                    }
                case "SORTGROUPCLAUSE":
                    {
                        var tleSortGroupRef = expressionData.SelectToken("tleSortGroupRef").Value<int>();
                        if (tleSortGroupRef > 0)
                        {
                            var query = expression.Parent.Parent.Parent;
                            foreach (var t in query.SelectToken("targetList").Children())
                            {
                                if (tleSortGroupRef == t.First.First.SelectToken("ressortgroupref").Value<int>())
                                {
                                    var expr = t.First.First.SelectToken("expr");
                                    return ResolveExpression(data, expr);
                                }
                            }
                        }
                        break;
                    }

            }
            return new UnknownExpression(data);
        }

        private void ProcessTargetEntries(QueryTreeDataInternal result, JToken query)
        {
            var setOperations = query.SelectToken("QUERY.setOperations");
            if (setOperations.Type == JTokenType.String) // it is empty
            {
                var targetEntries = query.SelectToken("QUERY.targetList");
                foreach (var child in targetEntries.Children())
                {
                    var targetEntry = child.SelectToken("TARGETENTRY");
                    var expr = targetEntry.SelectToken("expr");
                    var toAdd = ResolveExpression(result, expr);
                    result.TargetEntries.Add(toAdd);
                }
            }
            else
            {
                var rtes = setOperations.SelectTokens("..rtindex").Values<int>();
                foreach (var rteNumer in rtes)
                {
                    var rte = result.Rtes[rteNumer - 1];
                    var targetEntries = query.SelectToken("QUERY.targetList");
                    foreach (var child in targetEntries.Children())
                    {
                        var targetEntry = child.SelectToken("TARGETENTRY");
                        var expr = targetEntry.SelectToken("expr");
                        var toAdd = ResolveExpression(result, expr, rteNumer);
                        result.TargetEntries.Add(toAdd);
                    }
                }
            }
        }

        private void ProcessJoinTree(QueryTreeDataInternal result, JToken query)
        {
            var fromExpr = query.SelectToken("QUERY.jointree.FROMEXPR");
            if (fromExpr != null && fromExpr.Type != JTokenType.String) // is not empty
            {
                var quals = fromExpr.SelectToken("quals");
                if (quals.Type != JTokenType.String)
                {
                    result.WhereQuals.Add(ResolveExpression(result, quals));
                }
                var fromList = fromExpr.SelectToken("fromlist");
                if (fromList.Type != JTokenType.String)
                {
                    foreach (var joinQuals in fromList.SelectTokens("..quals"))
                    {
                        result.JoinQuals.Add(ResolveExpression(result, joinQuals));
                    }
                }
            }
        }

        private void ProcessHavingClause(QueryTreeDataInternal result, JToken query)
        {
            var having = query.SelectToken("QUERY.havingQual");
            if (having != null && having.Type != JTokenType.String)
            {
                result.HavingQuals.Add(ResolveExpression(result, having));
            }
        }

        private void ProcessSortClause(QueryTreeDataInternal result, JToken query)
        {
            var sortClause = query.SelectToken("QUERY.sortClause");
            if (sortClause != null && sortClause.Type != JTokenType.String)
            {
                foreach (var child in sortClause.Children())
                {
                    result.OrderByEntries.Add(ResolveExpression(result, child));
                }
            }
        }

        private void ProcessGroupClause(QueryTreeDataInternal result, JToken query)
        {
            var groupClause = query.SelectToken("QUERY.groupClause");
            if (groupClause!= null && groupClause.Type != JTokenType.String)
            {
                foreach (var child in groupClause.Children())
                {
                    result.GroupByEntries.Add(ResolveExpression(result, child));
                }
            }
        }
    }


    /*nodes.h
 *     CMD_UNKNOWN,
CMD_SELECT,					// select stmt 
CMD_UPDATE,					// update stmt 
CMD_INSERT,					// insert stmt 
CMD_DELETE,
CMD_UTILITY,				// cmds like create, destroy, copy, vacuum,
                             * etc. 
CMD_NOTHING					// dummy command for instead nothing rules
                             * with qual 
 */;
    enum CmdType
    {
        Unknown = 0,
        Select = 1,
        Update = 2,
        Insert = 3,
        Delete = 4,
        Utility = 5,
        Nothing = 6
    }

    /*parsenodes.h
RTE_RELATION,				// ordinary relation reference 
RTE_SUBQUERY,				// subquery in FROM 
RTE_JOIN,					// join 
RTE_FUNCTION,				// function in FROM 
RTE_TABLEFUNC,				// TableFunc(.., column list) 
RTE_VALUES,					// VALUES (<exprlist>), (<exprlist>), ... 
RTE_CTE,					// common table expr (WITH list element) 
RTE_NAMEDTUPLESTORE			// tuplestore, e.g. for AFTER triggers 
*/
    enum RteKind
    {
        Relation = 0,
        Subquery = 1,
        Join = 2,
        Function = 3,
        TableFunc = 4,
        Values = 5,
        CTE = 6,
        NamedTupleStore = 7
    }

    /*pg_class.h
#define RELKIND_RELATION		  'r'	// ordinary table
#define RELKIND_INDEX			  'i'	// secondary index
#define RELKIND_SEQUENCE		  'S'	// sequence object
#define RELKIND_TOASTVALUE	  't'	// for out-of-line values
#define RELKIND_VIEW			  'v'	// view
#define RELKIND_MATVIEW		  'm'	// materialized view
#define RELKIND_COMPOSITE_TYPE  'c'	// composite type
#define RELKIND_FOREIGN_TABLE   'f'	// foreign table
#define RELKIND_PARTITIONED_TABLE 'p' // partitioned table
*/
    enum RelKind
    {
        Unknown = 0,
        [EnumMember(Value = "r")]
        Relation = 1,
        [EnumMember(Value = "i")]
        Index = 2,
        [EnumMember(Value = "S")]
        Sequence = 3,
        [EnumMember(Value = "t")]
        ToastValue = 4,
        [EnumMember(Value = "v")]
        View = 5,
        [EnumMember(Value = "m")]
        MaterializedView = 6,
        [EnumMember(Value = "c")]
        CompositeType = 7,
        [EnumMember(Value = "f")]
        ForeignTable = 8,
        [EnumMember(Value = "p")]
        PartitionedTable = 9
    }

    // nodes.h
    // {
    //     /*
    //      * The canonical kinds of joins according to the SQL JOIN syntax. Only
    //      * these codes can appear in parser output (e.g., JoinExpr nodes).
    //      */
    //     JOIN_INNER,                 /* matching tuple pairs only */
    //     JOIN_LEFT,                  /* pairs + unmatched LHS tuples */
    //     JOIN_FULL,                  /* pairs + unmatched LHS + unmatched RHS */
    //     JOIN_RIGHT,                 /* pairs + unmatched RHS tuples */

    //     /*
    //      * Semijoins and anti-semijoins (as defined in relational theory) do not
    //      * appear in the SQL JOIN syntax, but there are standard idioms for
    //      * representing them (e.g., using EXISTS).  The planner recognizes these
    //      * cases and converts them to joins.  So the planner and executor must
    //      * support these codes.  NOTE: in JOIN_SEMI output, it is unspecified
    //      * which matching RHS row is joined to.  In JOIN_ANTI output, the row is
    //      * guaranteed to be null-extended.
    //      */
    //     JOIN_SEMI,                  /* 1 copy of each LHS row that has match(es) */
    //     JOIN_ANTI,                  /* 1 copy of each LHS row that has no match */

    //     /*
    //      * These codes are used internally in the planner, but are not supported
    //      * by the executor (nor, indeed, by most of the planner).
    //      */
    //     JOIN_UNIQUE_OUTER,          /* LHS path must be made unique */
    //     JOIN_UNIQUE_INNER           /* RHS path must be made unique */

    //    /*
    //     * We might need additional join types someday.
    //     */
    //}
    //JoinType;
    enum JoinType
    {
        Unknown = -1,
        Inner = 0,
        LeftOuter = 1,
        FullOuter = 2,
        RightOuter = 3,
        Semi = 4,
        AntiSemi = 5,
        UniqueOuter = 6,
        UniqueInner = 7
    }

    enum NullTestType
    {
        Unknown = -1,
        IsNull = 0,
        IsNotNull = 1
    }
}
