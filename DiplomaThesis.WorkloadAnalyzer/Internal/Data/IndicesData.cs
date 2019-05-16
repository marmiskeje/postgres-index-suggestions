using DiplomaThesis.DAL.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class IndicesData
    {
        private readonly HashSet<IndexDefinition> all = new HashSet<IndexDefinition>();
        private readonly Dictionary<long, ISet<IndexDefinition>> allPerStatement = new Dictionary<long, ISet<IndexDefinition>>();
        private readonly Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();
        private readonly Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> allCoveringPerQuery = new Dictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>>();

        public IReadOnlyCollection<IndexDefinition> All
        {
            get { return all; }
        }
        public IReadOnlyDictionary<long, ISet<IndexDefinition>> AllPerStatement
        {
            get { return allPerStatement; }
        }

        public IReadOnlyDictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> AllPerQuery
        {
            get { return allPerQuery; }
        }

        public IReadOnlyDictionary<NormalizedStatementQueryPair, ISet<IndexDefinition>> AllCoveringPerQuery
        {
            get { return allCoveringPerQuery; }
        }

        public IndicesData()
        {

        }

        private IndicesData(HashSet<IndexDefinition> all, Dictionary<long, ISet<IndexDefinition>> allPerStatement,
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
            all.ExceptWith(indices);
            HashSet<long> idsToRemove = new HashSet<long>();
            foreach (var statementId in allPerStatement.Keys)
            {
                allPerStatement[statementId].ExceptWith(indices);
                if (allPerStatement[statementId].Count == 0)
                {
                    idsToRemove.Add(statementId);
                }
            }
            foreach (var item in idsToRemove)
            {
                allPerStatement.Remove(item);
            }
            HashSet<NormalizedStatementQueryPair> queriesToRemove = new HashSet<NormalizedStatementQueryPair>();
            foreach (var query in allPerQuery.Keys)
            {
                allPerQuery[query].ExceptWith(indices);
                if (allPerQuery[query].Count == 0)
                {
                    queriesToRemove.Add(query);
                }
            }
            foreach (var item in queriesToRemove)
            {
                allPerQuery.Remove(item);
            }
            queriesToRemove.Clear();
            foreach (var query in allCoveringPerQuery.Keys)
            {
                allCoveringPerQuery[query].ExceptWith(indices);
                if (allCoveringPerQuery[query].Count == 0)
                {
                    queriesToRemove.Add(query);
                }
            }
            foreach (var item in queriesToRemove)
            {
                allCoveringPerQuery.Remove(item);
            }
        }

        public void RemoveExcept(IEnumerable<IndexDefinition> indices)
        {
            all.IntersectWith(indices);
            HashSet<long> idsToRemove = new HashSet<long>();
            foreach (var statementId in allPerStatement.Keys)
            {
                allPerStatement[statementId].IntersectWith(indices);
                if (allPerStatement[statementId].Count == 0)
                {
                    idsToRemove.Add(statementId);
                }
            }
            foreach (var item in idsToRemove)
            {
                allPerStatement.Remove(item);
            }
            HashSet<NormalizedStatementQueryPair> queriesToRemove = new HashSet<NormalizedStatementQueryPair>();
            foreach (var query in allPerQuery.Keys)
            {
                allPerQuery[query].IntersectWith(indices);
                if (allPerQuery[query].Count == 0)
                {
                    queriesToRemove.Add(query);
                }
            }
            foreach (var item in queriesToRemove)
            {
                allPerQuery.Remove(item);
            }
            queriesToRemove.Clear();
            foreach (var query in allCoveringPerQuery.Keys)
            {
                allCoveringPerQuery[query].IntersectWith(indices);
                if (allCoveringPerQuery[query].Count == 0)
                {
                    queriesToRemove.Add(query);
                }
            }
            foreach (var item in queriesToRemove)
            {
                allCoveringPerQuery.Remove(item);
            }
        }

        public IndicesData Clone()
        {
            return ToSubSetOf(All);
        }

        public IndicesData ToSubSetOf(IEnumerable<IndexDefinition> indices)
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
            foreach (var query in this.allCoveringPerQuery.Keys)
            {
                var toAdd = new HashSet<IndexDefinition>(this.allCoveringPerQuery[query].Intersect(indices));
                if (toAdd.Count > 0)
                {
                    allCoveringPerQuery.Add(query, toAdd);
                }
            }
            return new IndicesData(all, allPerStatement, allPerQuery, allCoveringPerQuery);
        }
    }
}
