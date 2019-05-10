using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    internal static class WorkloadValidator
    {
        public static (bool IsValid, String ErrorMessage) Validate(WorkloadData workload)
        {
            if (workload == null)
            {
                return (false, "Workload wasn´t provided.");
            }
            else if (workload.DatabaseID <= 0)
            {
                return (false, "Database identification wasn´t provided.");
            }
            else if (workload.Definition == null)
            {
                return (false, "Definition wasn´t provided.");
            }
            else if (String.IsNullOrWhiteSpace(workload.Name))
            {
                return (false, "Name wasn´t provided.");
            }
            return (true, String.Empty);
        }
    }
}
