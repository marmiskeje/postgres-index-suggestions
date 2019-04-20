using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class WorkloadAnalysis : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long WorkloadID { get; set; }
        public Workload Workload { get; set; }
        [Required]
        public DateTime PeriodFromDate { get; set; }
        [Required]
        public DateTime PeriodToDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public WorkloadAnalysisStateType State { get; set; }
        public List<VirtualEnvironment> VirtualEnvironments { get; set; }
        public List<WorkloadAnalysisRealStatementEvaluation> WorkloadAnalysisRealStatementEvaluations { get; set; }
    }

    public enum WorkloadAnalysisStateType
    {
        Created = 0,
        InProgress = 1,
        EndedSuccesfully = 2,
        EndedWithError = 3
    }
}
