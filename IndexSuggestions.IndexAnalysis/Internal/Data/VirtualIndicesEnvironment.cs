using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class VirtualIndicesEnvironment
    {
        public PossibleIndicesData PossibleIndices { get; }

        public PossibleIndicesData ImprovingPossibleIndices { get; } = new PossibleIndicesData();
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, QueryPlanNode> PlansPerStatement { get; } = new Dictionary<long, QueryPlanNode>();

        public VirtualIndicesEnvironment(PossibleIndicesData possibleIndices)
        {
            PossibleIndices = possibleIndices;
        }
    }
}
