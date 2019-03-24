using IndexSuggestions.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    class QueryPlanProvider
    {
#warning treba skontrolovat, preco nie je uplne korektne pre zlozite CTE
        public QueryPlanNode Provide(JObject jObject)
        {
            QueryPlanNode root = new QueryPlanNode();
            foreach (var plan in jObject.SelectTokens("..planTree"))
            {
                var planNode = plan.First.First;
                root.StartupCost = planNode.SelectToken("startup_cost").Value<decimal>();
                root.TotalCost = planNode.SelectToken("total_cost").Value<decimal>();
                if (planNode.SelectToken("indexid") != null)
                {
                    root.IndexId = planNode.SelectToken("indexid").Value<uint>();
                }
                var left = planNode.SelectToken("lefttree");
                LoadNode(root, left);
                var right = planNode.SelectToken("righttree");
                LoadNode(root, right);
            }
            return root;
        }

        private void LoadNode(QueryPlanNode parent, JToken jObject)
        {
            if (jObject.ToString() != "<>")
            {
                var planNode = jObject.First.First;
                QueryPlanNode node = new QueryPlanNode();
                node.StartupCost = planNode.SelectToken("startup_cost").Value<decimal>();
                node.TotalCost = planNode.SelectToken("total_cost").Value<decimal>();
                if (planNode.SelectToken("indexid") != null)
                {
                    node.IndexId = planNode.SelectToken("indexid").Value<uint>();
                }
                var left = planNode.SelectToken("lefttree");
                LoadNode(node, left);
                var right = planNode.SelectToken("righttree");
                LoadNode(node, right);
                parent.Plans.Add(node);
            }
        }
    }
}
