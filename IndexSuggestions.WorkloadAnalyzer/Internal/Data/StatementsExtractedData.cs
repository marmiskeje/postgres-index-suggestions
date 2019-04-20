using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class StatementsExtractedData
    {
        private readonly Dictionary<long, List<StatementQueryExtractedData>> normalizedStatementQueriesData = new Dictionary<long, List<StatementQueryExtractedData>>();
        private readonly Dictionary<NormalizedStatementQueryPair, StatementQueryExtractedData> dataPerQuery = new Dictionary<NormalizedStatementQueryPair, StatementQueryExtractedData>();

        public IDictionary<NormalizedStatementQueryPair, StatementQueryExtractedData> DataPerQuery
        {
            get { return dataPerQuery; }
        }
        public void Add(NormalizedStatement normalizedStatement, StatementQuery query, StatementQueryExtractedData queryExtractedData)
        {
            if (!normalizedStatementQueriesData.ContainsKey(normalizedStatement.ID))
            {
                normalizedStatementQueriesData.Add(normalizedStatement.ID, new List<StatementQueryExtractedData>());
            }
            normalizedStatementQueriesData[normalizedStatement.ID].Add(queryExtractedData);
            dataPerQuery.Add(new NormalizedStatementQueryPair(normalizedStatement.ID, query), queryExtractedData);
        }
    }
}
