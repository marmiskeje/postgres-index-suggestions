using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IExplainResult
    {
        string PlanJson { get; }
        QueryPlanNode Plan { get; }
        ISet<string> UsedIndexScanIndices { get; }
    }
}
