using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class GetStatementsStatisticsRequest
    {
        public uint DatabaseID { get; set; }
        public GetStatementsStatisticsFilter Filter { get; set; }
    }

    public class GetStatementsStatisticsFilter : BaseDateTimeFilter
    {
        public GetStatementsStatisticsCommandTypeFilter CommandType { get; set; }
    }

    public enum GetStatementsStatisticsCommandTypeFilter
    {
        Any = 0,
        Delete = 1,
        Insert = 2,
        Select = 3,
        Update = 4
    }
}
