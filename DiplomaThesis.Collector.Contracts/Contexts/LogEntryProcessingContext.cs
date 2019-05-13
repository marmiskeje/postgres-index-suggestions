using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public class LogEntryProcessingContext : IStatementProcessingContext
    {
        public LoggedEntry Entry { get; set; }
        public uint DatabaseID { get; set; }
        public DAL.Contracts.CollectorDatabaseConfiguration DatabaseCollectingConfiguration { get; set; }
        public LogEntryStatementData StatementData { get; private set; }
        public QueryPlanNode QueryPlan { get; set; }
        public QueryTreeData QueryTree { get; set; }

        public string Statement
        {
            get { return Entry.Statement; }
        }

        public DateTime ExecutionDate
        {
            get { return Entry.Timestamp; }
        }

        public LogEntryProcessingContext()
        {
            StatementData = new LogEntryStatementData();
        }
    }

    public class LogEntryStatementData
    {
        public string NormalizedStatement { get; set; }
        public DAL.Contracts.StatementQueryCommandType CommandType { get; set; }
        public string NormalizedStatementFingerprint { get; set; }
    }

    public class LogEntryStatementStatisticsData
    {
        public string NormalizedStatementFingerprint { get; set; }
        public string Statement { get; set; }
        public uint DatabaseID { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime ExecutionDate { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal? TotalCost { get; set; }
    }

    public class LogEntryStatementIndexStatisticsData
    {
        public string NormalizedStatementFingerprint { get; set; }
        public uint DatabaseID { get; set; }
        public uint IndexID { get; set; }
        public DateTime ExecutionDate { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class LogEntryStatementRelationStatisticsData
    {
        public string NormalizedStatementFingerprint { get; set; }
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public DateTime ExecutionDate { get; set; }
        public decimal TotalCost { get; set; }
    }
}
