using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface IStatementProcessingContext
    {
        DateTime ExecutionDate { get; }
        uint DatabaseID { get; }
        string Statement { get; }
    }
}
