using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public class QueryPlanNode
    {
        public decimal StartupCost { get; set; }
        public decimal TotalCost { get; set; }
        public QueryPlanScanOperation ScanOperation { get; set; }

        public List<QueryPlanNode> Plans { get; private set; }

        public QueryPlanNode()
        {
            Plans = new List<QueryPlanNode>();
        }
    }

    public abstract class QueryPlanScanOperation
    {

    }

    public class AnyIndexScanOperation : QueryPlanScanOperation
    {
        public uint IndexId { get; set; }
    }


    public class RelationSequenceScanOperation : QueryPlanScanOperation
    {
        public uint RelationId { get; set; }
    }
}
