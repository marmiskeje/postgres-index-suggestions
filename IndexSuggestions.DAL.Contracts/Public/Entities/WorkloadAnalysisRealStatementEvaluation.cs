using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class WorkloadAnalysisRealStatementEvaluation
    {
        [Required]
        public long WorkloadAnalysisID { get; set; }
        public WorkloadAnalysis WorkloadAnalysis { get; set; }
        [Required]
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        [Required]
        public long ExecutionPlanID { get; set; }
        public ExecutionPlan ExecutionPlan { get; set; }
    }
}
