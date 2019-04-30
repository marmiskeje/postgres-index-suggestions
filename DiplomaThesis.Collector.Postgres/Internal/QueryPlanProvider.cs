using DiplomaThesis.Common;
using DiplomaThesis.DBMS.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    class QueryPlanProvider
    {
#warning treba skontrolovat, preco nie je uplne korektne pre zlozite CTE
        public QueryPlanNode Provide(JObject jObject)
        {
            QueryTreeDataInternal queryTree = new QueryTreeDataInternal();
            ProcessRtes(queryTree, jObject);
            QueryPlanNode result = null;
            var plan = jObject.SelectToken("..planTree");
            if (plan != null)
            {
                result = LoadPlanNode(null, plan, queryTree);
            }
            return result ?? new QueryPlanNode();
        }

        private QueryPlanNode LoadPlanNode(QueryPlanNode parent, JToken jObject, QueryTreeDataInternal queryTree)
        {
            if (jObject.ToString() != "<>")
            {
                var planNode = jObject.First.First;
                var operationName = ((JProperty)jObject.First).Name.ToUpper();
                QueryPlanNode node = new QueryPlanNode();
                node.StartupCost = planNode.SelectToken("startup_cost").Value<decimal>();
                node.TotalCost = planNode.SelectToken("total_cost").Value<decimal>();
                if (operationName.Contains("INDEX") && planNode.SelectToken("indexid") != null)
                {
                    node.ScanOperation = new AnyIndexScanOperation() { IndexId = planNode.SelectToken("indexid").Value<uint>() };
                }
                else if (operationName == "SEQSCAN" && planNode.SelectToken("scanrelid") != null)
                {
                    var index = planNode.SelectToken("scanrelid").Value<int>();
                    var rte = queryTree.Rtes[index - 1];
                    if (rte.RelKind == RelKind.Relation)
                    {
                        node.ScanOperation = new RelationSequenceScanOperation() { RelationId = rte.RelId.Value };
                    }
                }
                var left = planNode.SelectToken("lefttree");
                LoadPlanNode(node, left, queryTree);
                var right = planNode.SelectToken("righttree");
                LoadPlanNode(node, right, queryTree);
                if (parent != null)
                {
                    parent.Plans.Add(node); 
                }
                return node;
            }
            return null;
        }

        private void ProcessRtes(QueryTreeDataInternal result, JToken query)
        {
            var rTable = query.SelectToken("..rtable");
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
                }
                result.Rtes.Add(rteToAdd);
            }
        }
    }
}
