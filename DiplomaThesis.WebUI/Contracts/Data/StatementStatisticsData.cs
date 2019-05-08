using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class StatementStatisticsData
    {
        public List<NormalizedStatementStatisticsTimeline> StatementTimeline { get; set; }
        public List<NormalizedStatementRelationStatistics> StatementRelationStatistics { get; set; }
        public List<NormalizedStatementIndexStatistics> StatementIndexStatistics { get; set; }
        public List<NormalizedStatementStatistics> SlowestRepresentatives { get; set; }
    }
}
