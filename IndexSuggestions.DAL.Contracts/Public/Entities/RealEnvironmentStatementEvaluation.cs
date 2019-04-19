using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class RealEnvironmentStatementEvaluation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long WorkloadAnalysisID { get; set; }
        public long NormalizedStatementID { get; set; }
        public long ExecutionPlanID { get; set; }
    }
}
