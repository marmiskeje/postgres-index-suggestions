using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class WorkloadStatementsData
    {
        private const decimal MOST_SIGNIFICANT_PORTION = 0.6m;
        private readonly Dictionary<long, NormalizedWorkloadStatement> all = new Dictionary<long, NormalizedWorkloadStatement>();
        private readonly Dictionary<long, NormalizedWorkloadStatement> allSelects = new Dictionary<long, NormalizedWorkloadStatement>();
        private readonly Dictionary<long, NormalizedWorkloadStatement> allUpdates = new Dictionary<long, NormalizedWorkloadStatement>();
        private readonly Dictionary<uint, List<NormalizedStatementQueryPair>> allQueriesByRelation = new Dictionary<uint, List<NormalizedStatementQueryPair>>();
        private readonly Dictionary<uint, List<NormalizedStatementQueryPair>> allSelectQueriesByRelation = new Dictionary<uint, List<NormalizedStatementQueryPair>>();
        private readonly Dictionary<uint, List<NormalizedStatementQueryPair>> mostSignificantSelectQueriesByRelation = new Dictionary<uint, List<NormalizedStatementQueryPair>>();
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public IReadOnlyDictionary<long, NormalizedWorkloadStatement> All
        {
            get { return all; }
        }
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public IReadOnlyDictionary<long, NormalizedWorkloadStatement> AllSelects
        {
            get { return allSelects; }
        }
        /// <summary>
        /// Key: NormalizedStatement.ID
        /// </summary>
        public IReadOnlyDictionary<long, NormalizedWorkloadStatement> AllUpdates
        {
            get { return allUpdates; }
        }
        /// <summary>
        /// Key: RelationID
        /// </summary>
        public IReadOnlyDictionary<uint, List<NormalizedStatementQueryPair>> AllQueriesByRelation
        {
            get { return allQueriesByRelation; }
        }
        /// <summary>
        /// Key: RelationID
        /// </summary>
        public IReadOnlyDictionary<uint, List<NormalizedStatementQueryPair>> AllSelectQueriesByRelation
        {
            get { return allSelectQueriesByRelation; }
        }
        /// <summary>
        /// Key: RelationID
        /// </summary>
        public IReadOnlyDictionary<uint, List<NormalizedStatementQueryPair>> MostSignificantSelectQueriesByRelation
        {
            get { return mostSignificantSelectQueriesByRelation; }
        }
        public WorkloadStatementsData(IEnumerable<NormalizedWorkloadStatement> data)
        {
            Dictionary<uint, decimal> allSelectQueriesTotalExecutionsByRelation = new Dictionary<uint, decimal>();
            foreach (var workloadStatement in data)
            {
                var statementCommandType = workloadStatement.NormalizedStatement.StatementDefinition.CommandType;
                all.Add(workloadStatement.NormalizedStatement.ID, workloadStatement);
                if (statementCommandType == StatementQueryCommandType.Select)
                {
                    allSelects.Add(workloadStatement.NormalizedStatement.ID, workloadStatement);
                }
                else if (statementCommandType == StatementQueryCommandType.Update)
                {
                    allUpdates.Add(workloadStatement.NormalizedStatement.ID, workloadStatement);
                }
                foreach (var query in workloadStatement.NormalizedStatement.StatementDefinition.IndependentQueries)
                {
                    var queryStatement = new NormalizedStatementQueryPair(workloadStatement.NormalizedStatement.ID, query);
                    foreach (var r in query.Relations)
                    {
                        if (query.CommandType == StatementQueryCommandType.Select)
                        {
                            if (!mostSignificantSelectQueriesByRelation.ContainsKey(r.ID))
                            {
                                allSelectQueriesByRelation.Add(r.ID, new List<NormalizedStatementQueryPair>());
                            }
                            allSelectQueriesByRelation[r.ID].Add(queryStatement);
                            if (!allSelectQueriesTotalExecutionsByRelation.ContainsKey(r.ID))
                            {
                                allSelectQueriesTotalExecutionsByRelation.Add(r.ID, 0);
                            }
                            allSelectQueriesTotalExecutionsByRelation[r.ID] += workloadStatement.TotalExecutionsCount; 
                        }
                        if (!allQueriesByRelation.ContainsKey(r.ID))
                        {
                            allQueriesByRelation.Add(r.ID, new List<NormalizedStatementQueryPair>());
                        }
                        allQueriesByRelation[r.ID].Add(queryStatement);
                    }
                }
            }
            PrepareMostSignificantData(allSelectQueriesTotalExecutionsByRelation);
        }
        private void PrepareMostSignificantData(Dictionary<uint, decimal> allSelectQueriesTotalExecutionsByRelation)
        {
            foreach (var kv in AllSelectQueriesByRelation)
            {
                var queries = kv.Value;
                queries.Sort((x, y) =>
                {
                    var xExecCounts = allSelects[x.NormalizedStatementID].TotalExecutionsCount;
                    var yExecCounts = allSelects[x.NormalizedStatementID].TotalExecutionsCount;
                    return xExecCounts.CompareTo(yExecCounts);
                });
            }
            foreach (var kv in AllSelectQueriesByRelation)
            {
                var relationID = kv.Key;
                var sortedQueries = kv.Value;
                decimal threshold = allSelectQueriesTotalExecutionsByRelation[kv.Key] * MOST_SIGNIFICANT_PORTION;
                decimal cumulatedValue = 0;
                int itemsCountToTake = 0;
                for (int i = 0; i < sortedQueries.Count; i++)
                {
                    cumulatedValue += allSelects[sortedQueries[i].NormalizedStatementID].TotalExecutionsCount;
                    if (cumulatedValue > threshold)
                    {
                        itemsCountToTake = i+1;
                        break;
                    }
                }
                mostSignificantSelectQueriesByRelation.Add(relationID, new List<NormalizedStatementQueryPair>(sortedQueries.Take(itemsCountToTake)));
            }
        }
    }
}
