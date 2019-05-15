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
        public Dictionary<long, VirtualEnvironmentStatementEvaluation> StatementsEvaluation { get; } = new Dictionary<long, VirtualEnvironmentStatementEvaluation>();

        public Dictionary<IndexDefinition, VirtualIndicesEnvironmentIndexEvaluation> IndicesEvaluation { get; } = new Dictionary<IndexDefinition, VirtualIndicesEnvironmentIndexEvaluation>();

        public VirtualIndicesEnvironment(IndicesData possibleIndices)
        {
            PossibleIndices = possibleIndices;
        }
    }

    internal class VirtualEnvironmentStatementEvaluation
    {
        public IExplainResult ExecutionPlan { get; set; }
        public decimal LocalImprovementRatio { get; set; }
        public decimal GlobalImprovementRatio { get; set; }
    }

    internal class VirtualIndicesEnvironmentIndexEvaluation
    {
        public decimal ImprovementRatio { get; set; }
    }
}
