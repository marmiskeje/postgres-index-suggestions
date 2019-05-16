using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Required]
        public decimal LocalImprovementRatio { get; set; }
        [Required]
        public decimal GlobalImprovementRatio { get; set; }
        [Required]
        public string AffectingIndicesData { get; set; }
        [NotMapped]
        public HashSet<long> AffectingIndices { get; set; } = new HashSet<long>();
        [Required]
        public string UsedIndicesData { get; set; }
        [NotMapped]
        public HashSet<long> UsedIndices { get; set; } = new HashSet<long>();
    }
}
