using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class GetTotalRelationStatisticsRequest
    {
        public uint RelationID { get; set; }
        public BaseDateTimeFilter Filter { get; set; }
    }
}
