using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedWorkloadStatement
    {
        public NormalizedStatement NormalizedStatement { get; set; }
        public NormalizedStatementStatistics RepresentativeStatistics { get; set; }
        public long TotalExecutionsCount { get; set; }
    }
}
