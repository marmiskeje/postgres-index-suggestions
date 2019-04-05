using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.Collector
{
    internal class PublishTotalViewStatisticsCommand : ChainableCommand
    {
        private readonly IStatementProcessingContext context;
        private readonly IDatabaseDependencyHierarchyProvider dependencyHierarchyProvider;
        private readonly IStatisticsProcessingDataAccumulator statisticsAccumulator;

        public PublishTotalViewStatisticsCommand(IStatementProcessingContext context, IDatabaseDependencyHierarchyProvider dependencyHierarchyProvider,
                                                 IStatisticsProcessingDataAccumulator statisticsAccumulator)
        {
            this.context = context;
            this.dependencyHierarchyProvider = dependencyHierarchyProvider;
            this.statisticsAccumulator = statisticsAccumulator;
        }

        protected override void OnExecute()
        {
            var dependencyHierarchy = dependencyHierarchyProvider.ProvideForDatabaseFilteredForViewOccurence(context.DatabaseID);
            if (dependencyHierarchy != null)
            {
                var statement = context.Statement.ToLower();
                foreach (var v in dependencyHierarchy.Views)
                {
                    CheckAndReportViewStatistics(statement, v);
                }
                foreach (var p in dependencyHierarchy.StoredProcedures)
                {
                    CheckAndReportViewStatistics(statement, p);
                }
            }
        }

        private void CheckAndReportViewStatistics(string statement, IDependencyHierarchyObject obj)
        {
            var regex = new Regex(obj.SearchOccurencePattern);
            foreach (var m in regex.Matches(statement))
            {
                ReportViewStatistics(obj);
            }
        }

        private void ReportViewStatistics(IDependencyHierarchyObject obj)
        {
            if (obj.ObjectType == DependencyHierarchyObjectType.View)
            {
                statisticsAccumulator.PublishViewStatistics(new StatementViewStatisticsData()
                {
                    DatabaseID = obj.DatabaseID,
                    ExecutionDate = context.ExecutionDate,
                    ViewID = obj.ID
                }); 
            }
            foreach (var d in obj.Dependencies)
            {
                ReportViewStatistics(d);
            }
        }
    }
}
