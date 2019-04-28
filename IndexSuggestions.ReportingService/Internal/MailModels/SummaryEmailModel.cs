using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.ReportingService
{
    public class SummaryEmailModel
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public List<SummaryEmailDatabaseInfo> Databases { get; } = new List<SummaryEmailDatabaseInfo>();
        public string ToDateString(DateTime date)
        {
            return String.Format("{0} {1}", date.ToShortDateString(), date.ToLongTimeString());
        }
    }

    public class SummaryEmailDatabaseInfo
    {
        public string Name { get; set; }
        public List<SummaryEmailTopStatement> TopExecutedStatements { get; } = new List<SummaryEmailTopStatement>();
        public List<SummaryEmailTopStatement> TopSlowestStatements { get; } = new List<SummaryEmailTopStatement>();
        public List<SummaryEmailTopAliveRelation> TopAliveRelations { get; } = new List<SummaryEmailTopAliveRelation>();
    }

    public class SummaryEmailTopStatement
    {
        public string Statement { get; set; }
        public long ExecutionCount { get; set; }
        public string MaxDuration { get; set; }
        public string AvgDuration { get; set; }
        public string MinDuration { get; set; }
        public string TotalDuration { get; set; }
    }

    public class SummaryEmailTopAliveRelation
    {
        public string Name { get; set; }
        public long SeqScansCount { get; set; }
        public long IndexScansCount { get; set; }
        public long TuplesInsertCount { get; set; }
        public long TuplesUpdateCount { get; set; }
        public long TuplesDeleteCount { get; set; }
        public decimal TotalCount { get; set; }
    }
}
