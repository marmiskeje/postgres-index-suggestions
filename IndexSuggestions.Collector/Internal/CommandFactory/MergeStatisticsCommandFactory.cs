using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.Collector
{
    internal class MergeStatisticsCommandFactory : IMergeStatisticsCommandFactory
    {
        private readonly IRepositoriesFactory dalRepositories;
        public MergeStatisticsCommandFactory(IRepositoriesFactory dalRepositories)
        {
            this.dalRepositories = dalRepositories;
        }
        public IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementStatisticsRepository();
            return new LoadStatisticsForMergeCommand<long, NormalizedStatementStatistics>(context, (from, to) => repository.GetAllGroupedByStatement(from, to));
        }

        public IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementStatisticsContext context)
        {
            return new MergeStatisticsCommand<long, NormalizedStatementStatistics>(context, () => new NormalizedStatementStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementStatisticsRepository();
            return new SaveMergedStatisticsCommand<long, NormalizedStatementStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }
    }
}
