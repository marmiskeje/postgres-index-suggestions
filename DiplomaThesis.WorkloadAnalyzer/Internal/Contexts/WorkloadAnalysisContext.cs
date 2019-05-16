using DiplomaThesis.DAL.Contracts;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class WorkloadAnalysisContext
    {
        public WorkloadAnalysis WorkloadAnalysis { get; set; }
        public Workload Workload { get; set; }
        public DBMS.Contracts.IDatabase Database { get; set; }
        public WorkloadStatementsData StatementsData { get; set; }
        public WorkloadRelationsData RelationsData { get; set; }
        public StatementsExtractedData StatementsExtractedData { get; } = new StatementsExtractedData();
        public IndicesDesignData IndicesDesignData { get; } = new IndicesDesignData();
        public HPartitioningsDesignData HPartitioningDesignData { get; } = new HPartitioningsDesignData();
        public Dictionary<long, IExplainResult> RealExecutionPlansForStatements { get; } = new Dictionary<long, IExplainResult>();
    }

    internal class IndicesDesignData
    {
        public IndicesData ExistingIndices { get; } = new IndicesData();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public IndicesData PossibleIndices { get; } = new IndicesData();
        public HashSet<VirtualIndicesEnvironment> Environments { get; } = new HashSet<VirtualIndicesEnvironment>();
        public Dictionary<IndexDefinition, long> PossibleIndexSizes { get; } = new Dictionary<IndexDefinition, long>();
        public Dictionary<IndexDefinition, Dictionary<string, long>> PossibleIndexFilters { get; } = new Dictionary<IndexDefinition, Dictionary<string, long>>();
    }

    internal class HPartitioningsDesignData
    {
        public Dictionary<AttributeData, HPartitioningAttributeDefinition> AttributesPartitioning { get; } = new Dictionary<AttributeData, HPartitioningAttributeDefinition>();
        public List<VirtualHPartitioningEnvironment> Environments { get; } = new List<VirtualHPartitioningEnvironment>();
    }
}
