using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class NormalizedStatementQueryPair
    {
        public long NormalizedStatementID { get; }
        public StatementQuery Query { get; }
        public NormalizedStatementQueryPair(long normalizedStatementID, StatementQuery query)
        {
            NormalizedStatementID = normalizedStatementID;
            Query = query;
        }
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is NormalizedStatementQueryPair))
            {
                return false;
            }
            var tmp = (NormalizedStatementQueryPair)obj;
            return Query == tmp.Query;
        }
        public override int GetHashCode()
        {
            return Query.GetHashCode();
        }
    }
}
