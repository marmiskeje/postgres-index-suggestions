using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class WorkloadAnalysisContext
    {
        public WorkloadAnalysis WorkloadAnalysis { get; set; }
        public Workload Workload { get; set; }
        public DBMS.Contracts.IDatabase Database { get; set; }
        public IndicesDesignData IndicesDesignData { get; } = new IndicesDesignData();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public Dictionary<long, NormalizedWorkloadStatement> Statements { get; } = new Dictionary<long, NormalizedWorkloadStatement>();
    }

    internal class IndicesDesignData
    {
        public HashSet<IndexDefinition> ExistingIndices { get; } = new HashSet<IndexDefinition>();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public Dictionary<long, IExplainResult> RealExecutionPlansForStatements { get; } = new Dictionary<long, IExplainResult>();
        public StatementsExtractedData StatementsExtractedData { get; } = new StatementsExtractedData();
        public PossibleIndicesData PossibleIndices { get; } = new PossibleIndicesData();
        public List<VirtualIndicesEnvironment> IndicesEnvironments { get; } = new List<VirtualIndicesEnvironment>();
        public Dictionary<IndexDefinition, long> PossibleIndexSizes { get; } = new Dictionary<IndexDefinition, long>();
        public Dictionary<IndexDefinition, Dictionary<string, long>> PossibleIndexFilters { get; } = new Dictionary<IndexDefinition, Dictionary<string, long>>();
    }
}
