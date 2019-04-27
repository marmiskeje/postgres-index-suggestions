using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class VirtualHPartitioningEnvironment
    {
        public HPartitioningDefinition Partitioning { get; }
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, IExplainResult> PlansPerStatement { get; } = new Dictionary<long, IExplainResult>();

        public bool IsImproving { get; set; }

        public VirtualHPartitioningEnvironment(HPartitioningDefinition partitioning)
        {
            Partitioning = partitioning;
        }
    }
}
