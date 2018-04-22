using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedWorkloadStatementsRepository : IBaseRepository<long, NormalizedWorkloadStatement>
    {
        NormalizedWorkloadStatement Get(long statementId, long workloadId, bool useCache = false);
        IList<NormalizedWorkloadStatement> GetAllByWorkloadId(long workloadId, NormalizedWorkloadStatementFilter filter);
    }

    public class NormalizedWorkloadStatementFilter
    {
        public StatementQueryCommandType? CommandType { get; set; }
        public int? MinExecutionsCount { get; set; }
    }
}
