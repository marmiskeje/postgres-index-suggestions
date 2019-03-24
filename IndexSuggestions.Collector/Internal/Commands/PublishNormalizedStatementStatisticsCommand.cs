using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class PublishNormalizedStatementStatisticsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementStatisticsCommand(LogEntryProcessingContext context, IStatementDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            statementDataAccumulator.PublishNormalizedStatementStatistics(new LogEntryStatementStatisticsData()
            {
                ApplicationName = context.Entry.ApplicationName,
                DatabaseID = context.DatabaseID,
                Duration = context.Entry.Duration,
                ExecutionDate = context.Entry.Timestamp,
                NormalizedStatementFingerprint = context.StatementData.NormalizedStatementFingerprint,
                Statement = context.Entry.Statement,
                UserName = context.Entry.UserName
            });
        }
    }
}
