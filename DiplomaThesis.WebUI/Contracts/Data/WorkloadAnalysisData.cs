using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class WorkloadAnalysisData
    {
        public long ID { get; set; }
        public string ErrorMessage { get; set; }
        public WorkloadData Workload { get; set; }
        public DateTime PeriodFromDate { get; set; }
        public DateTime PeriodToDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int State { get; set; }
    }
}
