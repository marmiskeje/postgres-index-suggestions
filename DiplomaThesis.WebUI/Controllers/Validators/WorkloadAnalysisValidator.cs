using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    internal static class WorkloadAnalysisValidator
    {
        public static (bool IsValid, String ErrorMessage) Validate(CreateWorkloadAnalysisRequest createRequest)
        {
            if (createRequest == null)
            {
                return (false, "Input data wasn´t provided.");
            }
            else if (createRequest.WorkloadID <= 0)
            {
                return (false, "Workload identification wasn´t provided.");
            }
            else if (!(createRequest.PeriodFromDate < createRequest.PeriodToDate))
            {
                return (false, "Date period is not valid.");
            }
            return (true, String.Empty);
        }
    }
}
