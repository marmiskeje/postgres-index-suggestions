using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class GetStoredProcedureStatisticsRequest
    {
        public uint StoredProcedureID { get; set; }
        public BaseDateTimeFilter Filter { get; set; }
    }
}
