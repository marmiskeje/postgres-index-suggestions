using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class GetWorkloadAnalysesRequest
    {
        public uint DatabaseID { get; set; }
        public BaseDateTimeFilter Filter { get; set; }
    }
}
