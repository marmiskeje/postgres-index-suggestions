using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class VirtualHPartitioningEnvironment
    {
        public HPartitioningDefinition Partitioning { get; }
        /// <summary>
        /// Key: NormalizedStatementID
        /// </summary>
        public Dictionary<long, VirtualEnvironmentStatementEvaluation> StatementsEvaluation { get; } = new Dictionary<long, VirtualEnvironmentStatementEvaluation>();

        public VirtualHPartitioningEnvironmentHPartitioningEvaluation Evaluation { get; } = new VirtualHPartitioningEnvironmentHPartitioningEvaluation();
        public bool IsImproving { get; set; }

        public VirtualHPartitioningEnvironment(HPartitioningDefinition partitioning)
        {
            Partitioning = partitioning;
        }
    }

    internal class VirtualHPartitioningEnvironmentHPartitioningEvaluation
    {
        public decimal ImprovementRatio { get; set; }
    }
}
