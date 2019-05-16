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
        private string Identificator { get; }
        public IndicesData ImprovingPossibleIndices { get; } = new IndicesData();
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, VirtualEnvironmentStatementEvaluation> StatementsEvaluation { get; } = new Dictionary<long, VirtualEnvironmentStatementEvaluation>();

        public Dictionary<IndexDefinition, VirtualIndicesEnvironmentIndexEvaluation> IndicesEvaluation { get; } = new Dictionary<IndexDefinition, VirtualIndicesEnvironmentIndexEvaluation>();

        public VirtualIndicesEnvironment(IndicesData possibleIndices)
        {
            PossibleIndices = possibleIndices;
            Identificator = string.Join("_", PossibleIndices.All.Select(x => x.ToString()).OrderBy(x => x));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is VirtualIndicesEnvironment))
            {
                return false;
            }
            var tmp = (VirtualIndicesEnvironment)obj;
            return String.Equals(Identificator, tmp.Identificator);
        }

        public override int GetHashCode()
        {
            return Identificator.GetHashCode();
        }
    }

    internal class VirtualEnvironmentStatementEvaluation
    {
        public IExplainResult ExecutionPlan { get; set; }
        public decimal LocalImprovementRatio { get; set; }
        public decimal GlobalImprovementRatio { get; set; }
        public HashSet<IndexDefinition> AffectingIndices { get; } = new HashSet<IndexDefinition>();
        public HashSet<IndexDefinition> UsedIndices { get; } = new HashSet<IndexDefinition>();
    }

    internal class VirtualIndicesEnvironmentIndexEvaluation
    {
        public decimal ImprovementRatio { get; set; }
    }
}
