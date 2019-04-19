using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    internal class VirtualEnvironmentStatementEvaluation
    {
        public long VirtualEnvironmentID { get; set; }
        public long NormalizedStatementID { get; set; }
        public long ExecutionPlanID { get; set; }
    }
}
