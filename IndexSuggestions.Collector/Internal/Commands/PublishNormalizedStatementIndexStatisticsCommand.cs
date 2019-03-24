using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    class PublishNormalizedStatementIndexStatisticsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementIndexStatisticsCommand(LogEntryProcessingContext context, IStatementDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            if (context.QueryPlan != null)
            {
                PublishIndexStatistic(context.QueryPlan);
            }
        }

        private void PublishIndexStatistic(QueryPlanNode node)
        {
            if (node.IndexId.HasValue)
            {
                statementDataAccumulator.PublishNormalizedStatementIndexStatistics(new LogEntryStatementIndexStatisticsData()
                {
                    DatabaseID = context.DatabaseID,
                    ExecutionDate = context.Entry.Timestamp,
                    IndexID = node.IndexId.Value,
                    NormalizedStatementFingerprint = context.StatementData.NormalizedStatementFingerprint,
                    TotalCost = node.TotalCost
                });
            }
            foreach (var item in node.Plans)
            {
                PublishIndexStatistic(item);
            }
        }
    }
}
