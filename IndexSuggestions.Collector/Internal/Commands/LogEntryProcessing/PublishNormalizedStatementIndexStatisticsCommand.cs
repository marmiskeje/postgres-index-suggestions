using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    class PublishNormalizedStatementIndexStatisticsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementIndexStatisticsCommand(LogEntryProcessingContext context, IStatementsProcessingDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            if (context.QueryPlan != null)
            {
                PublishIndexStatistics(context.QueryPlan);
            }
        }

        private void PublishIndexStatistics(QueryPlanNode plan)
        {
            switch (plan.ScanOperation)
            {
                case AnyIndexScanOperation indexScan:
                    statementDataAccumulator.PublishNormalizedStatementIndexStatistics(new LogEntryStatementIndexStatisticsData()
                    {
                        DatabaseID = context.DatabaseID,
                        ExecutionDate = context.Entry.Timestamp,
                        IndexID = indexScan.IndexId,
                        NormalizedStatementFingerprint = context.StatementData.NormalizedStatementFingerprint,
                        TotalCost = plan.TotalCost
                    });
                    break;
            }
            foreach (var item in plan.Plans)
            {
                PublishIndexStatistics(item);
            }
        }
    }
}
