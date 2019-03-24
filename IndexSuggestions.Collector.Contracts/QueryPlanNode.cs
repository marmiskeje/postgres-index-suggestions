using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class QueryPlanNode
    {
        public decimal StartupCost { get; set; }
        public decimal TotalCost { get; set; }
        public uint? IndexId { get; set; }

        public List<QueryPlanNode> Plans { get; private set; }

        public QueryPlanNode()
        {
            Plans = new List<QueryPlanNode>();
        }
    }

}
