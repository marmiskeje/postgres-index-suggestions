using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class WorkloadAnalysisDetailData
    {
        public Dictionary<long, WorkloadAnalysisEnvironment> IndicesEnvironments { get; } = new Dictionary<long, WorkloadAnalysisEnvironment>();
        public Dictionary<long, WorkloadAnalysisEnvironment> HPartitioningsEnvironments { get; } = new Dictionary<long, WorkloadAnalysisEnvironment>();
        public Dictionary<long, WorkloadAnalysisEnvironmentIndexExtended> Indices { get; } = new Dictionary<long, WorkloadAnalysisEnvironmentIndexExtended>();
    }

    public class WorkloadAnalysisEnvironment
    {
        public long ID { get; set; }
        public Dictionary<long, WorkloadAnalysisEnvironmentIndex> Indices { get; } = new Dictionary<long, WorkloadAnalysisEnvironmentIndex>();
        public Dictionary<long, WorkloadAnalysisEnvironmentHPartitioning> HPartitionings { get; } = new Dictionary<long, WorkloadAnalysisEnvironmentHPartitioning>();
    }

    public class WorkloadAnalysisEnvironmentIndex
    {
        public long ID { get; set; }
        public decimal ImprovementRatio { get; set; }
    }

    public class WorkloadAnalysisEnvironmentIndexExtended
    {
        public long ID { get; set; }
        public uint RelationID { get; set; }
        public string Name { get; set; }
        public string CreateDefinition { get; set; }
        public long Size { get; set; }
        public Dictionary<string, long> Filters { get; } = new Dictionary<string, long>();
    }

    public class WorkloadAnalysisEnvironmentHPartitioning
    {
        public long ID { get; set; }
        public uint RelationID { get; set; }
        public string PartitioningStatement { get; set; }
        public List<string> PartitionStatements { get; } = new List<string>();
        public decimal ImprovementRatio { get; set; }
    }
}
