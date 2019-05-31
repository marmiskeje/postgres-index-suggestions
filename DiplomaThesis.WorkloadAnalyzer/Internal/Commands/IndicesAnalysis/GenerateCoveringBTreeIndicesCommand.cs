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
    internal class GenerateCoveringBTreeIndicesCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;

        public GenerateCoveringBTreeIndicesCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }

        protected override void OnExecute()
        {
            foreach (var kv in context.IndicesDesignData.PossibleIndices.AllPerQuery)
            {
                var query = kv.Key.Query;
                var statementId = kv.Key.NormalizedStatementID;
                var statement = context.StatementsData.All[statementId].NormalizedStatement;
                var possibleBaseIndices = kv.Value;
                var queryExtractedData = context.StatementsExtractedData.DataPerQuery[kv.Key];
                List<IndexDefinition> coveringIndices = new List<IndexDefinition>();
                foreach (var possibleBaseIndex in possibleBaseIndices)
                {
                    if (!context.Workload.Definition.Relations.ForbiddenValues.Contains(possibleBaseIndex.Relation.ID))
                    {
                        if (TryCreateCoveringIndex(query, queryExtractedData, possibleBaseIndex, out var coveringIndex) != CreateCoveringIndexResult.NotPossible)
                        {
                            coveringIndices.Add(coveringIndex);
                        }
                    }
                }
                context.IndicesDesignData.PossibleIndices.TryAddPossibleCoveringIndices(coveringIndices, statement, query);
            }
            foreach (var kv in context.IndicesDesignData.ExistingIndices.AllPerQuery)
            {
                var query = kv.Key.Query;
                var statementId = kv.Key.NormalizedStatementID;
                var statement = context.StatementsData.All[statementId].NormalizedStatement;
                var possibleBaseIndices = kv.Value;
                var queryExtractedData = context.StatementsExtractedData.DataPerQuery[kv.Key];
                List<IndexDefinition> coveringIndices = new List<IndexDefinition>();
                foreach (var possibleBaseIndex in possibleBaseIndices)
                {
                    if (!context.Workload.Definition.Relations.ForbiddenValues.Contains(possibleBaseIndex.Relation.ID))
                    {
                        if (TryCreateCoveringIndex(query, queryExtractedData, possibleBaseIndex, out var coveringIndex) == CreateCoveringIndexResult.CreatedNew)
                        {
                            coveringIndices.Add(coveringIndex);
                        }
                    }
                }
                context.IndicesDesignData.PossibleIndices.TryAddPossibleCoveringIndices(coveringIndices, statement, query);
            }
        }

        private enum CreateCoveringIndexResult
        {
            NotPossible = 0,
            CreatedSameAsBaseIndex = 1,
            CreatedNew = 2
        }

        private CreateCoveringIndexResult TryCreateCoveringIndex(DAL.Contracts.StatementQuery query, StatementQueryExtractedData queryExtractedData, IndexDefinition baseIndex, out IndexDefinition coveringIndex)
        {
            var allWhereAttributes = queryExtractedData.WhereAttributes.All.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var bTreeWhereAttributes = queryExtractedData.WhereAttributes.BTreeApplicable.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var allJoinAttributes = queryExtractedData.JoinAttributes.All.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var bTreeJoinAttributes = queryExtractedData.JoinAttributes.BTreeApplicable.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var allGroupByAttributes = queryExtractedData.GroupByAttributes.All.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var bTreeGroupByAttributes = queryExtractedData.GroupByAttributes.BTreeApplicable.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var allOrderByAttributes = queryExtractedData.OrderByAttributes.All.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var bTreeOrderByAttributes = queryExtractedData.OrderByAttributes.BTreeApplicable.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            var allProjectionAttributes = queryExtractedData.ProjectionAttributes.All.Where(x => x.Relation.ID == baseIndex.Relation.ID);
            if (allWhereAttributes.Count() == bTreeWhereAttributes.Count()
                && allJoinAttributes.Count() == bTreeJoinAttributes.Count()
                && allGroupByAttributes.Count() == bTreeGroupByAttributes.Count()
                && allOrderByAttributes.Count() == bTreeOrderByAttributes.Count()
                && query.CommandType != DAL.Contracts.StatementQueryCommandType.Insert)
            {
                var includeAttributes = new HashSet<AttributeData>(bTreeWhereAttributes);
                includeAttributes.UnionWith(bTreeJoinAttributes);
                includeAttributes.UnionWith(bTreeGroupByAttributes);
                includeAttributes.UnionWith(bTreeOrderByAttributes);
                includeAttributes.UnionWith(allProjectionAttributes);
                includeAttributes.ExceptWith(baseIndex.Attributes);
                List<AttributeData> includeSortedAttributes = new List<AttributeData>(includeAttributes.OrderBy(x => x.CardinalityIndicator));
                coveringIndex = new IndexDefinition(baseIndex.StructureType, baseIndex.Relation, baseIndex.Attributes, includeSortedAttributes);
                return baseIndex.IncludeAttributes.Count != coveringIndex.IncludeAttributes.Count ? CreateCoveringIndexResult.CreatedNew : CreateCoveringIndexResult.CreatedSameAsBaseIndex; 
            }
            else
            {
                coveringIndex = null;
                return CreateCoveringIndexResult.NotPossible;
            }
        }
    }
}