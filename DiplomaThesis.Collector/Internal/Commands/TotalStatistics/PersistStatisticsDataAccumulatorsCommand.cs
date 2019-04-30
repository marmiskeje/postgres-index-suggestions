using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DiplomaThesis.Collector
{
    internal class PersistStatisticsDataAccumulatorsCommand : ChainableCommand
    {
        private readonly IStatisticsProcessingDataAccumulator statisticsAccumulator;
        private readonly IRepositoriesFactory repositories;
        public PersistStatisticsDataAccumulatorsCommand(IStatisticsProcessingDataAccumulator statisticsAccumulator, IRepositoriesFactory repositories)
        {
            this.statisticsAccumulator = statisticsAccumulator;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            var state = statisticsAccumulator.ProvideState();
            using (var scope = new TransactionScope())
            {
                foreach (var r in state.TotalRelationStatistics)
                {
                    r.Value.ForEach(x => PersistTotalRelationStatistics(x));
                }
                foreach (var i in state.TotalIndexStatistics)
                {
                    i.Value.ForEach(x => PersistTotalIndexStatistics(x));
                }
                foreach (var p in state.TotalProceduresStatistics)
                {
                    p.Value.ForEach(x => PersistTotalProcedureStatistics(x));
                }
                foreach (var p in state.TotalViewStatistics)
                {
                    p.Value.ForEach(x => PersistTotalViewStatistics(x));
                }
                scope.Complete();
            }
            statisticsAccumulator.ClearState();
        }

        private void PersistTotalRelationStatistics(TotalRelationStatistics newStatistics)
        {
            var repository = repositories.GetTotalRelationStatisticsRepository();
            var uniqueKey = new TotalRelationStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                RelationID = newStatistics.RelationID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                TotalRelationStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }
        private void PersistTotalIndexStatistics(TotalIndexStatistics newStatistics)
        {
            var repository = repositories.GetTotalIndexStatisticsRepository();
            var uniqueKey = new TotalIndexStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                IndexID = newStatistics.IndexID,
                RelationID = newStatistics.RelationID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                TotalIndexStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }
        private void PersistTotalProcedureStatistics(TotalStoredProcedureStatistics newStatistics)
        {
            var repository = repositories.GetTotalStoredProcedureStatisticsRepository();
            var uniqueKey = new TotalStoredProcedureStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                ProcedureID = newStatistics.ProcedureID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                TotalStoredProcedureStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }

        private void PersistTotalViewStatistics(TotalViewStatistics newStatistics)
        {
            var repository = repositories.GetTotalViewStatisticsRepository();
            var uniqueKey = new TotalViewStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                ViewID = newStatistics.ViewID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                TotalViewStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }
    }
}
