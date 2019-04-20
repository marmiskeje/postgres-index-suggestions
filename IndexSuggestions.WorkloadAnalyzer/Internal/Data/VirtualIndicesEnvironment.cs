using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class VirtualIndicesEnvironment
    {
        public PossibleIndicesData PossibleIndices { get; }

        public PossibleIndicesData ImprovingPossibleIndices { get; } = new PossibleIndicesData();
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, IExplainResult> PlansPerStatement { get; } = new Dictionary<long, IExplainResult>();

        public VirtualIndicesEnvironment(PossibleIndicesData possibleIndices)
        {
            PossibleIndices = possibleIndices;
        }
    }
}
