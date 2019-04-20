using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class VirtualEnvironment : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long WorkloadAnalysisID { get; set; }
        public WorkloadAnalysis WorkloadAnalysis { get; set; }
        public List<VirtualEnvironmentPossibleIndex> VirtualEnvironmentPossibleIndices { get; set; }
        public List<VirtualEnvironmentStatementEvaluation> VirtualEnvironmentStatementEvaluations { get; set; }
        public List<VirtualEnvironmentPossibleCoveringIndex> VirtualEnvironmentPossibleCoveringIndices { get; set; }
    }
}
