using System.Linq;
using System.Collections.Generic;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    /// <summary>
    /// For each possible index generate for query covering one.
    /// Also for each existing index generate for query covering one.
    /// </summary>
    internal class GenerateCoveringIndicesCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;

        public GenerateCoveringIndicesCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }

        protected override void OnExecute()
        {
            foreach (var kv in context.IndicesDesignData.PossibleIndices.AllPerQuery)
            {
                var query = kv.Key.Query;
                var statementId = kv.Key.NormalizedStatementID;
                var statement = context.StatementsData.AllSelects[statementId].NormalizedStatement;
                var possibleBaseIndices = kv.Value;
                var queryExtractedData = context.StatementsExtractedData.DataPerQuery[kv.Key];
                List<IndexDefinition> coveringIndices = new List<IndexDefinition>();
                foreach (var possibleBaseIndex in possibleBaseIndices)
                {
                    TryCreateCoveringIndex(queryExtractedData, possibleBaseIndex, out var coveringIndex);
                    coveringIndices.Add(coveringIndex);
                }
                context.IndicesDesignData.PossibleIndices.TryAddPossibleCoveringIndices(coveringIndices, statement, query);
            }
            foreach (var kv in context.IndicesDesignData.ExistingIndices.AllPerQuery)
            {
                var query = kv.Key.Query;
                var statementId = kv.Key.NormalizedStatementID;
                var statement = context.StatementsData.AllSelects[statementId].NormalizedStatement;
                var possibleBaseIndices = kv.Value;
                var queryExtractedData = context.StatementsExtractedData.DataPerQuery[kv.Key];
                List<IndexDefinition> coveringIndices = new List<IndexDefinition>();
                foreach (var possibleBaseIndex in possibleBaseIndices)
                {
                    if (TryCreateCoveringIndex(queryExtractedData, possibleBaseIndex, out var coveringIndex))
                    {
                        coveringIndices.Add(coveringIndex);
                    }
                }
                context.IndicesDesignData.PossibleIndices.TryAddPossibleCoveringIndices(coveringIndices, statement, query);
            }
        }

        /// <summary>
        /// Returns true, if covering index is different than base index.
        /// </summary>
        private bool TryCreateCoveringIndex(StatementQueryExtractedData queryExtractedData, IndexDefinition baseIndex, out IndexDefinition coveringIndex)
        {
            var includeAttributes = new HashSet<AttributeData>(queryExtractedData.WhereAttributes);
            includeAttributes.UnionWith(queryExtractedData.JoinAttributes);
            includeAttributes.UnionWith(queryExtractedData.GroupByAttributes);
            includeAttributes.UnionWith(queryExtractedData.OrderByAttributes);
            includeAttributes.UnionWith(queryExtractedData.ProjectionAttributes);
            includeAttributes.ExceptWith(baseIndex.Attributes);
            List<AttributeData> includeSortedAttributes = new List<AttributeData>(includeAttributes.OrderBy(x => x.CardinalityIndicator));
            coveringIndex = new IndexDefinition(baseIndex.StructureType, baseIndex.Relation, baseIndex.Attributes, includeSortedAttributes);
            return baseIndex.IncludeAttributes.Count != coveringIndex.IncludeAttributes.Count;
        }
    }
}