using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class GetWorkloadAnalysisDetailForEnvRequest
    {
        public long WorkloadAnalysisID { get; set; }
        public long EnvironmentID { get; set; }
    }
}
