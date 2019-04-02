using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    class PublishNormalizedStatementRelationStatisticsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        public PublishNormalizedStatementRelationStatisticsCommand(LogEntryProcessingContext context, IStatementsProcessingDataAccumulator statementDataAccumulator)
        {
            this.context = context;
            this.statementDataAccumulator = statementDataAccumulator;
        }
        protected override void OnExecute()
        {
            if (context.QueryPlan != null)
            {
                PublishRelationStatistics(context.QueryPlan);
            }
        }

        private void PublishRelationStatistics(QueryPlanNode plan)
        {
            switch (plan.ScanOperation)
            {
                case RelationSequenceScanOperation relationSeqScan:
                    statementDataAccumulator.PublishNormalizedStatementRelationStatistics(new LogEntryStatementRelationStatisticsData()
                    {
                        DatabaseID = context.DatabaseID,
                        ExecutionDate = context.Entry.Timestamp,
                        RelationID = relationSeqScan.RelationId,
                        NormalizedStatementFingerprint = context.StatementData.NormalizedStatementFingerprint,
                        TotalCost = plan.TotalCost
                    });
                    break;
            }
            foreach (var item in plan.Plans)
            {
                PublishRelationStatistics(item);
            }
        }
    }
}
