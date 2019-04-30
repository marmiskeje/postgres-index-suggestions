using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.Collector
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

        public IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementRelationStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementRelationStatisticsRepository();
            return new LoadStatisticsForMergeCommand<long, NormalizedStatementRelationStatistics>(context, (from, to) => repository.GetAllGroupedByStatement(from, to));
        }

        public IChainableCommand LoadStatisticsForMergeCommand(MergeNormalizedStatementIndexStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementIndexStatisticsRepository();
            return new LoadStatisticsForMergeCommand<long, NormalizedStatementIndexStatistics>(context, (from, to) => repository.GetAllGroupedByStatement(from, to));
        }

        public IChainableCommand LoadStatisticsForMergeCommand(MergeTotalRelationStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalRelationStatisticsRepository();
            return new LoadStatisticsForMergeCommand<uint, TotalRelationStatistics>(context, (from, to) => repository.GetAllGroupedByRelation(from, to));
        }

        public IChainableCommand LoadStatisticsForMergeCommand(MergeTotalIndexStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalIndexStatisticsRepository();
            return new LoadStatisticsForMergeCommand<uint, TotalIndexStatistics>(context, (from, to) => repository.GetAllGroupedByIndex(from, to));
        }

        public IChainableCommand LoadStatisticsForMergeCommand(MergeTotalStoredProcedureStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalStoredProcedureStatisticsRepository();
            return new LoadStatisticsForMergeCommand<uint, TotalStoredProcedureStatistics>(context, (from, to) => repository.GetAllGroupedByProcedure(from, to));
        }

        public IChainableCommand LoadStatisticsForMergeCommand(MergeTotalViewStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalViewStatisticsRepository();
            return new LoadStatisticsForMergeCommand<uint, TotalViewStatistics>(context, (from, to) => repository.GetAllGroupedByView(from, to));
        }

        public IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementStatisticsContext context)
        {
            return new MergeStatisticsCommand<long, NormalizedStatementStatistics>(context, () => new NormalizedStatementStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context)
        {
            return new MergeStatisticsCommand<long, NormalizedStatementRelationStatistics>(context, () => new NormalizedStatementRelationStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context)
        {
            return new MergeStatisticsCommand<long, NormalizedStatementIndexStatistics>(context, () => new NormalizedStatementIndexStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeTotalRelationStatisticsContext context)
        {
            return new MergeStatisticsCommand<uint, TotalRelationStatistics>(context, () => new TotalRelationStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeTotalIndexStatisticsContext context)
        {
            return new MergeStatisticsCommand<uint, TotalIndexStatistics>(context, () => new TotalIndexStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeTotalStoredProcedureStatisticsContext context)
        {
            return new MergeStatisticsCommand<uint, TotalStoredProcedureStatistics>(context, () => new TotalStoredProcedureStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand MergeStatisticsCommand(MergeTotalViewStatisticsContext context)
        {
            return new MergeStatisticsCommand<uint, TotalViewStatistics>(context, () => new TotalViewStatisticsSampler(context.DateTimeSelector, null));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementStatisticsRepository();
            return new SaveMergedStatisticsCommand<long, NormalizedStatementStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementRelationStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementRelationStatisticsRepository();
            return new SaveMergedStatisticsCommand<long, NormalizedStatementRelationStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeNormalizedStatementIndexStatisticsContext context)
        {
            var repository = dalRepositories.GetNormalizedStatementIndexStatisticsRepository();
            return new SaveMergedStatisticsCommand<long, NormalizedStatementIndexStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeTotalRelationStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalRelationStatisticsRepository();
            return new SaveMergedStatisticsCommand<uint, TotalRelationStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeTotalIndexStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalIndexStatisticsRepository();
            return new SaveMergedStatisticsCommand<uint, TotalIndexStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeTotalStoredProcedureStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalStoredProcedureStatisticsRepository();
            return new SaveMergedStatisticsCommand<uint, TotalStoredProcedureStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }

        public IChainableCommand SaveMergedStatisticsCommand(MergeTotalViewStatisticsContext context)
        {
            var repository = dalRepositories.GetTotalViewStatisticsRepository();
            return new SaveMergedStatisticsCommand<uint, TotalViewStatistics>(context, x => repository.Remove(x), x => repository.Create(x));
        }
    }
}
