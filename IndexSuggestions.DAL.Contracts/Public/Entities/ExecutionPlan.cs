using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class ExecutionPlan : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string Json { get; set; }
        [Required]
        public decimal TotalCost { get; set; }
        public List<VirtualEnvironmentStatementEvaluation> VirtualEnvironmentStatementEvaluations { get; set; }
        public List<WorkloadAnalysisRealStatementEvaluation> WorkloadAnalysisRealStatementEvaluations { get; set; }
    }
}
