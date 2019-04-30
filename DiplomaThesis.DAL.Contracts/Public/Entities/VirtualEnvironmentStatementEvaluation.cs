using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class VirtualEnvironmentStatementEvaluation
    {
        [Required]
        public long VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }
        [Required]
        public long NormalizedStatementID { get; set; }

        public NormalizedStatement NormalizedStatement { get; set; }
        [Required]
        public long ExecutionPlanID { get; set; }
        public ExecutionPlan ExecutionPlan { get; set; }
    }
}
