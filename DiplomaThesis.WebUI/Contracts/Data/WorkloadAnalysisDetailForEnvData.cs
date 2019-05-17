using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class WorkloadAnalysisDetailForEnvData
    {
        public long EnvironmentID { get; set; }
        public Dictionary<long, WorkloadAnalysisDetailForEnvStatementData> Statements { get; } = new Dictionary<long, WorkloadAnalysisDetailForEnvStatementData>();
        public Dictionary<long, WorkloadAnalysisDetailForEnvStatementEvaluationData> EvaluatedStatements { get; } = new Dictionary<long, WorkloadAnalysisDetailForEnvStatementEvaluationData>();
    }

    public class WorkloadAnalysisDetailForEnvStatementData
    {
        public string Statement { get; set; }
        public long TotalExecutionsCount { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class WorkloadAnalysisDetailForEnvStatementEvaluationData
    {
        public decimal TotalCost { get; set; }
        public HashSet<long> AffectingIndices { get; } = new HashSet<long>();
        public HashSet<long> UsedIndices { get; } = new HashSet<long>();
        public decimal LocalImprovementRatio { get; set; }
        public decimal GlobalImprovementRatio { get; set; }
        public HashSet<long> CoveringIndices { get; } = new HashSet<long>();
    }
}
