using System.Linq;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class GenerateCoveringBtreeIndicesCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;

        public GenerateCoveringBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }

        protected override void OnExecute()
        {
            foreach (var kv in context.IndicesDesignData.PossibleIndices.AllPerQuery)
            {
                var query = kv.Key.Query;
                var statementId = kv.Key.NormalizedStatementID;
                var statement = context.Statements[statementId].NormalizedStatement;
                var possibleBaseIndices = kv.Value;
                var queryExtractedData = context.IndicesDesignData.StatementsExtractedData.DataPerQuery[kv.Key];
                List<IndexDefinition> coveringIndices = new List<IndexDefinition>();
                foreach (var possibleBaseIndex in possibleBaseIndices)
                {
                    var includeAttributes = new HashSet<IndexAttribute>(queryExtractedData.WhereAttributes);
                    includeAttributes.UnionWith(queryExtractedData.JoinAttributes);
                    includeAttributes.UnionWith(queryExtractedData.GroupByAttributes);
                    includeAttributes.UnionWith(queryExtractedData.OrderByAttributes);
                    includeAttributes.UnionWith(queryExtractedData.ProjectionAttributes);
                    includeAttributes.ExceptWith(possibleBaseIndex.Attributes);
                    List<IndexAttribute> includeSortedAttributes = new List<IndexAttribute>(includeAttributes.OrderBy(x => x.CardinalityIndicator));
                    coveringIndices.Add(new IndexDefinition(possibleBaseIndex.StructureType, possibleBaseIndex.Relation, possibleBaseIndex.Attributes, includeSortedAttributes));
                }
                context.IndicesDesignData.PossibleIndices.TryAddPossibleCoveringIndices(coveringIndices, statement, query);
            }
        }
    }
}