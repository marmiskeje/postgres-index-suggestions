using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class VirtualIndicesEnvironment
    {
        public IndicesData PossibleIndices { get; }

        public IndicesData ImprovingPossibleIndices { get; } = new IndicesData();
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, IExplainResult> PlansPerStatement { get; } = new Dictionary<long, IExplainResult>();

        public VirtualIndicesEnvironment(IndicesData possibleIndices)
        {
            PossibleIndices = possibleIndices;
        }
    }
}
