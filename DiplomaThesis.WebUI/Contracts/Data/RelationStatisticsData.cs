using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class RelationStatisticsData
    {
        public List<TotalRelationStatistics> TotalRelationStatistics { get; set; }
        public List<RelationSummaryStatementStatistics> RelationSummaryStatementStatistics { get; set; }
    }
}
