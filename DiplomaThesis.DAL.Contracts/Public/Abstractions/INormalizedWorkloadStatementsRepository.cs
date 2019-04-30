using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedWorkloadStatementsRepository
    {
        IEnumerable<NormalizedWorkloadStatement> GetWorkloadStatements(Workload workload, DateTime fromInclusive, DateTime toExclusive);
    }
}
