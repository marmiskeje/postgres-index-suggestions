using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
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
            foreach (var plan in context.QueryPlans)
            {
                PublishIndexStatistics(plan);
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
