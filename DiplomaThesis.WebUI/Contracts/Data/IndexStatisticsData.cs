using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class IndexStatisticsData
    {
        public List<TotalIndexStatistics> TotalIndexStatistics { get; set; }
        public List<IndexSummaryStatementStatistics> IndexSummaryStatementStatistics { get; set; }
    }
}
