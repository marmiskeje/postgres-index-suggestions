using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class PublishNormalizedStatementStatisticsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementStatisticsCommand(LogEntryProcessingContext context, IStatementsProcessingDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            var stats = new LogEntryStatementStatisticsData()
            {
                ApplicationName = context.Entry.ApplicationName,
                DatabaseID = context.DatabaseID,
                Duration = context.Entry.Duration,
                ExecutionDate = context.Entry.Timestamp,
                NormalizedStatementFingerprint = context.StatementData.NormalizedStatementFingerprint,
                Statement = context.Entry.Statement,
                UserName = context.Entry.UserName
            };
            if (context.QueryPlan != null)
            {
                stats.TotalCost = context.QueryPlan.TotalCost;
            }
            statementDataAccumulator.PublishNormalizedStatementStatistics(stats);
        }
    }
}
