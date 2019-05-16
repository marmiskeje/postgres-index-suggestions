using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class CreateWorkloadAnalysisRequest
    {
        public long WorkloadID { get; set; }
        public DateTime PeriodFromDate { get; set; }
        public DateTime PeriodToDate { get; set; }
        public Dictionary<uint, uint> RelationReplacements { get; set; }
    }
}
