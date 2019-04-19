using IndexSuggestions.DAL.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class PossibleIndicesData
    {
        //private readonly Dictionary<uint, ISet<IndexDefinition>> relationPossibleIndices = new Dictionary<uint, ISet<IndexDefinition>>();
        private readonly HashSet<IndexDefinition> all = new HashSet<IndexDefinition>();
        private readonly Dictionary<long, ISet<IndexDefinition>> allPerStatement = new Dictionary<long, ISet<IndexDefinition>>();
        private readonly Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();
        private readonly Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allCoveringPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();
        /*
        public IReadOnlyDictionary<uint, ISet<IndexDefinition>> PossibleIndicesPerRelation
        {
            get { return relationPossibleIndices; }
        }
        */
        public IReadOnlyDictionary<long, ISet<IndexDefinition>> AllPerStatement
        {
            get { return allPerStatement; }
        }

        public IReadOnlyDictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> AllPerQuery
        {
            get { return allPerQuery; }
        }

        public IReadOnlyCollection<IndexDefinition> All
        {
            get { return all; }
        }

        public PossibleIndicesData()
        {

        }

        private PossibleIndicesData(HashSet<IndexDefinition> all, Dictionary<long, ISet<IndexDefinition>> allPerStatement,
                                    Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allPerQuery,
                                    Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allCoveringPerQuery)
        {
            this.all = all;
            this.allPerStatement = allPerStatement;
            this.allPerQuery = allPerQuery;
            this.allCoveringPerQuery = allCoveringPerQuery;
        }
        public void TryAddPossibleIndices(IEnumerable<IndexDefinition> indexDefinitions, NormalizedStatement normalizedStatement, StatementQuery query)
        {
            if (!allPerStatement.ContainsKey(normalizedStatement.ID))
            {
                allPerStatement.Add(normalizedStatement.ID, new HashSet<IndexDefinition>());
            }
            var queryKey = new NormalizedStatementQueryPair(normalizedStatement.ID, query);
            if (!allPerQuery.ContainsKey(queryKey))
            {
                allPerQuery.Add(queryKey, new HashSet<IndexDefinition>());
            }
            foreach (var indexDefinition in indexDefinitions)
            {
                all.Add(indexDefinition);
                /*
                if (!relationPossibleIndices.ContainsKey(indexDefinition.Relation.ID))
                {
                    relationPossibleIndices.Add(indexDefinition.Relation.ID, new HashSet<IndexDefinition>());
                }
                IndexDefinition indexDefinitionRepresentant = null;
                if (!relationPossibleIndices[indexDefinition.Relation.ID].TryGetValue(indexDefinition, out indexDefinitionRepresentant))
                {
                    relationPossibleIndices[indexDefinition.Relation.ID].Add(indexDefinition);
                    indexDefinitionRepresentant = indexDefinition;
                }
                */

                allPerStatement[normalizedStatement.ID].Add(indexDefinition);
                allPerQuery[queryKey].Add(indexDefinition);
            }
        }

        public void TryAddPossibleCoveringIndices(IEnumerable<IndexDefinition> indexDefinitions, NormalizedStatement normalizedStatement, StatementQuery query)
        {
            TryAddPossibleIndices(indexDefinitions, normalizedStatement, query);
            var queryKey = new NormalizedStatementQueryPair(normalizedStatement.ID, query);
            if (!allCoveringPerQuery.ContainsKey(queryKey))
            {
                allCoveringPerQuery.Add(queryKey, new HashSet<IndexDefinition>());
            }
            foreach (var indexDefinition in indexDefinitions)
            {
                allCoveringPerQuery[queryKey].Add(indexDefinition);
            }
        }

        public void Remove(IEnumerable<IndexDefinition> indices)
        {
            /*
            foreach (var relationId in relationPossibleIndices.Keys)
            {
                relationPossibleIndices[relationId].ExceptWith(indices);
            }
            */
            all.ExceptWith(indices);
            foreach (var statementId in allPerStatement.Keys)
            {
                allPerStatement[statementId].ExceptWith(indices);
            }
            foreach (var query in allPerQuery.Keys)
            {
                allPerQuery[query].ExceptWith(indices);
            }
            foreach (var query in allPerQuery.Keys)
            {
                allCoveringPerQuery[query].ExceptWith(indices);
            }
        }

        public void RemoveExcept(IEnumerable<IndexDefinition> indices)
        {
            /*
            foreach (var relationId in relationPossibleIndices.Keys)
            {
                relationPossibleIndices[relationId].IntersectWith(indices);
            }
            */
            all.IntersectWith(indices);
            foreach (var statementId in allPerStatement.Keys)
            {
                allPerStatement[statementId].IntersectWith(indices);
            }
            foreach (var query in allPerQuery.Keys)
            {
                allPerQuery[query].IntersectWith(indices);
            }
            foreach (var query in allPerQuery.Keys)
            {
                allCoveringPerQuery[query].IntersectWith(indices);
            }
        }

        public PossibleIndicesData Clone()
        {
            return ToSubSetOf(All);
        }

        public PossibleIndicesData ToSubSetOf(IEnumerable<IndexDefinition> indices)
        {
            var all = new HashSet<IndexDefinition>(this.all.Intersect(indices));
            var allPerStatement = new Dictionary<long, ISet<IndexDefinition>>();
            var allPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();
            var allCoveringPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();
            foreach (var statementId in this.allPerStatement.Keys)
            {
                var toAdd = new HashSet<IndexDefinition>(this.allPerStatement[statementId].Intersect(indices));
                if (toAdd.Count > 0)
                {
                    allPerStatement.Add(statementId, toAdd);
                }
            }
            foreach (var query in this.allPerQuery.Keys)
            {
                var toAdd = new HashSet<IndexDefinition>(this.allPerQuery[query].Intersect(indices));
                if (toAdd.Count > 0)
                {
                    allPerQuery.Add(query, toAdd);
                }
            }
            foreach (var query in allCoveringPerQuery.Keys)
            {
                var toAdd = new HashSet<IndexDefinition>(this.allCoveringPerQuery[query].Intersect(indices));
                if (toAdd.Count > 0)
                {
                    allCoveringPerQuery.Add(query, toAdd);
                }
            }
            return new PossibleIndicesData(this.all, allPerStatement, allPerQuery, allCoveringPerQuery);
        }
    }
}
