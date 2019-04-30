using DiplomaThesis.DBMS.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class ExplainResultProvider
    {
        public ExplainResult Provide(string jsonPlan)
        {
            var result = new ExplainResult();
            result.PlanJson = jsonPlan;
            var jObject = new JObject(jsonPlan);
            var plan = jObject.SelectToken("..Plan");
            if (plan != null)
            {
                HashSet<string> allIndices = new HashSet<string>();
                result.Plan = LoadPlanNode(null, plan, allIndices);
                foreach (var i in allIndices)
                {
                    result.UsedIndexScanIndices.Add(i);
                }
            }
            return result;
        }

        private QueryPlanNode LoadPlanNode(QueryPlanNode parent, JToken jObject, ISet<string> allIndices)
        {
            QueryPlanNode node = new QueryPlanNode();
            var nodeType = jObject.SelectToken("['Node Type']").Value<string>().ToUpper();
            if (nodeType.Contains("INDEX") && jObject.SelectToken("['Index Name']") != null)
            {
                var indexName = jObject.SelectToken("['Index Name']").Value<string>();
                //var relationName = jObject.SelectToken("['Relation Name']").Value<string>();
                allIndices.Add(indexName);
            }
            /*
            else if (nodeType == "Seq Scan" && jObject.SelectToken("['Relation Name']") != null)
            {
                var relationName = jObject.SelectToken("['Relation Name']").Value<string>();
            }
            */
            var plans = jObject.SelectToken("Plans");
            if (plans != null)
            {
                foreach (var plan in plans.Children())
                {
                    node.Plans.Add(LoadPlanNode(node, plan, allIndices));
                } 
            }
            return node;
        }
    }
}
