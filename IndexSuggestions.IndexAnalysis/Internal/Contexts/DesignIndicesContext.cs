using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class DesignIndicesContext
    {
        public long WorkloadID { get; set; }
        public DateTime DataFilterFromDate { get; set; }
        public DateTime DataFilterToDate { get; set; }
        public Workload Workload { get; set; }
        public DBMS.Contracts.IDatabase Database { get; set; }
        public HashSet<IndexDefinition> ExistingIndices { get; } = new HashSet<IndexDefinition>();
        public IndicesDesignData IndicesDesignData { get; } = new IndicesDesignData();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public Dictionary<long, NormalizedWorkloadStatement> Statements { get; } = new Dictionary<long, NormalizedWorkloadStatement>();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public Dictionary<long, IExplainResult> RealExecutionPlansForStatements { get; } = new Dictionary<long, IExplainResult>();
    }

    internal class IndicesDesignData
    {
        public StatementsExtractedData StatementsExtractedData { get; } = new StatementsExtractedData();
        public PossibleIndicesData PossibleIndices { get; } = new PossibleIndicesData();
        public List<VirtualIndicesEnvironment> IndicesEnvironments { get; } = new List<VirtualIndicesEnvironment>();
        public Dictionary<IndexDefinition, long> PossibleIndexSizes { get; } = new Dictionary<IndexDefinition, long>();
        public Dictionary<IndexDefinition, HashSet<string>> PossibleIndexFilters { get; } = new Dictionary<IndexDefinition, HashSet<string>>();
    }
}
