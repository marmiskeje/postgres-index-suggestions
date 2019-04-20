using System;
using System.Linq;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class GenerateBaseBtreeIndicesCommand : ChainableCommand
    {
        private const int MULTICOLUMN_ATTRIBUTES_MAX_COUNT = 5;
        private readonly WorkloadAnalysisContext context;

        public GenerateBaseBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }

        protected override void OnExecute()
        {
            foreach (var kv in context.IndicesDesignData.StatementsExtractedData.DataPerQuery)
            {
                var query = kv.Key.Query;
                var statement = context.Statements[kv.Key.NormalizedStatementID].NormalizedStatement;
                var data = kv.Value;
                GenerateIndices(data, statement, query);
            }
        }

        private class IndexAttributeCardinalityComparer : IComparer<IndexAttribute>
        {
            private static readonly Lazy<IComparer<IndexAttribute>> instance = new Lazy<IComparer<IndexAttribute>>(new IndexAttributeCardinalityComparer());

            public static IComparer<IndexAttribute> Default
            {
                get { return instance.Value; }
            }
            public int Compare(IndexAttribute x, IndexAttribute y)
            {
                return x.CardinalityIndicator.CompareTo(y.CardinalityIndicator);
            }
        }

        private void GenerateIndices(StatementQueryExtractedData extractedData, NormalizedStatement normalizedStatement, StatementQuery query)
        {
            var possibleIndices = context.IndicesDesignData.PossibleIndices;
            var whereJoinAttributesToUse = GroupAttributesByRelationAndPrepare(extractedData.WhereAttributes.Union(extractedData.JoinAttributes));
            var groupByAttributesToUse = GroupAttributesByRelationAndPrepare(extractedData.GroupByAttributes);
            var orderByAttributesToUse = GroupAttributesByRelationAndPrepare(extractedData.OrderByAttributes);

            // order by indices
            foreach (var kv in orderByAttributesToUse)
            {
                var relation = kv.Key;
                var attributes = kv.Value;
                var permutations = GenerateAllPermutations(relation, attributes);
                possibleIndices.TryAddPossibleIndices(permutations.AllPermutations, normalizedStatement, query);
            }
            // group by indices
            foreach (var kv in groupByAttributesToUse)
            {
                var relation = kv.Key;
                var attributes = kv.Value;
                var permutations = GenerateAllPermutations(relation, attributes);
                possibleIndices.TryAddPossibleIndices(permutations.AllPermutations, normalizedStatement, query);
            }
            // where U join, where U join + group by, where U join + order by
            // note: where U join + group by + order by is not needed (any atrribute in order by must be already in group by)
            foreach (var kv in whereJoinAttributesToUse)
            {
                var relation = kv.Key;
                var attributes = kv.Value;
                var whereJoinPermutations = GenerateAllPermutations(relation, attributes);
                // where U join
                possibleIndices.TryAddPossibleIndices(whereJoinPermutations.AllPermutations, normalizedStatement, query);
                if (whereJoinPermutations.LastGenerationNumber < MULTICOLUMN_ATTRIBUTES_MAX_COUNT)
                {
                    var maxAttributesCountToConcat = MULTICOLUMN_ATTRIBUTES_MAX_COUNT - whereJoinPermutations.LastGenerationNumber;
                    // where U join + group by
                    if (groupByAttributesToUse.ContainsKey(relation))
                    {
                        var attributesToConcat = groupByAttributesToUse[relation].Except(whereJoinAttributesToUse[relation]).OrderBy(x => x.CardinalityIndicator).Take(maxAttributesCountToConcat).ToHashSet();
                        var permutationsToConcat = GenerateAllPermutations(relation, attributesToConcat);
                        var newPermutations = ConcatPermutations(whereJoinPermutations.AllPermutations, permutationsToConcat.AllPermutations);
                        possibleIndices.TryAddPossibleIndices(newPermutations, normalizedStatement, query);
                    }
                    // where U join + order by
                    if (orderByAttributesToUse.ContainsKey(relation))
                    {
                        var attributesToConcat = orderByAttributesToUse[relation].Except(whereJoinAttributesToUse[relation]).OrderBy(x => x.CardinalityIndicator).Take(maxAttributesCountToConcat).ToHashSet();
                        var permutationsToConcat = GenerateAllPermutations(relation, attributesToConcat);
                        var newPermutations = ConcatPermutations(whereJoinPermutations.AllPermutations, permutationsToConcat.AllPermutations);
                        possibleIndices.TryAddPossibleIndices(newPermutations, normalizedStatement, query);
                    }
                }
            }
        }

        private ISet<IndexDefinition> ConcatPermutations(ISet<IndexDefinition> first, ISet<IndexDefinition> second)
        {
            if (first.Count == 0)
            {
                return second;
            }
            else if (second.Count == 0)
            {
                return first;
            }
            ISet<IndexDefinition> result = new HashSet<IndexDefinition>();
            foreach (var f in first)
            {
                foreach (var s in second)
                {
                    var mergedAttributes = new List<IndexAttribute>(f.Attributes);
                    mergedAttributes.AddRange(s.Attributes);
                    var newDefinition = new IndexDefinition(f.StructureType, f.Relation, mergedAttributes, null);
                    result.Add(newDefinition);
                }
            }
            return result;
        }

        private class IndexDefinitionPermutations
        {
            public Dictionary<int, HashSet<IndexDefinition>> PermutationsByGeneration { get; } = new Dictionary<int, HashSet<IndexDefinition>>();
            public int LastGenerationNumber
            {
                get { return PermutationsByGeneration.Count; }
            }
            public ISet<IndexDefinition> AllPermutations
            {
                get { return PermutationsByGeneration.Values.SelectMany(x => x).ToHashSet(); }
            }
        }

        private IndexDefinitionPermutations GenerateAllPermutations(IndexRelation relation, ISet<IndexAttribute> attributes)
        {
            var result = new IndexDefinitionPermutations();
            for (int i = 1; i <= attributes.Count; i++)
            {
                result.PermutationsByGeneration.Add(i, new HashSet<IndexDefinition>());
                var permutations = PermuteUtils.Permute(attributes, i);
                foreach (var p in permutations)
                {
                    var indexToAdd = new IndexDefinition(IndexStructureType.BTree, relation, p, null);
                    result.PermutationsByGeneration[i].Add(indexToAdd);
                }
            }
            return result;
        }

        private Dictionary<IndexRelation, SortedSet<IndexAttribute>> GroupAttributesByRelationAndPrepare(IEnumerable<IndexAttribute> attributes)
        {
            return attributes.GroupBy(x => x.Relation).ToDictionary(x => x.Key, x => new SortedSet<IndexAttribute>(x.OrderBy(y => y.CardinalityIndicator).Take(MULTICOLUMN_ATTRIBUTES_MAX_COUNT), IndexAttributeCardinalityComparer.Default));
        }

    }
}