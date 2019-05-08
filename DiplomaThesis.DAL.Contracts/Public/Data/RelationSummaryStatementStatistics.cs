using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class RelationSummaryStatementStatistics
    {
        public long NormalizedStatementID { get; set; }
        public string NormalizedStatement { get; set; }
        public long SeqScansCount { get; set; }
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public decimal AvgCost { get; set; }
    }
}
