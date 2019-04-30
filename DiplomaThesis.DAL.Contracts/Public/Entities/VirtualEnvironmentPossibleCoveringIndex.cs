using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class VirtualEnvironmentPossibleCoveringIndex
    {
        [Required]
        public long VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }
        [Required]
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        [Required]
        public long PossibleIndexID { get; set; }
        public PossibleIndex PossibleIndex { get; set; }
    }
}
