using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace IndexSuggestions.Collector
{
    internal class PersistDataAccumulatorsCommand : ChainableCommand
    {
        private readonly IStatementDataAccumulator statementDataAccumulator;
        private readonly IRepositoriesFactory repositories;
        private readonly ILastProcessedLogEntryEvidence processedLogEntryEvidence;
        public PersistDataAccumulatorsCommand(IStatementDataAccumulator statementDataAccumulator, IRepositoriesFactory repositories,
                                              ILastProcessedLogEntryEvidence processedLogEntryEvidence)
        {
            this.statementDataAccumulator = statementDataAccumulator;
            this.repositories = repositories;
            this.processedLogEntryEvidence = processedLogEntryEvidence;
        }
        protected override void OnExecute()
        {
            var state = statementDataAccumulator.ProvideState();
            var statementsRepository = repositories.GetNormalizedStatementsRepository();
            using (var scope = new TransactionScope())
            {
                foreach (var s in state.Statements)
                {
                    var fingerprint = s.Key;
                    var newStatement = s.Value;
                    var oldStatement = statementsRepository.GetByStatementFingerprint(newStatement.StatementFingerprint, true);
                    PersistStatementIfNeeded(oldStatement, newStatement);
                    if (state.StatementStatistics.ContainsKey(newStatement.StatementFingerprint))
                    {
                        foreach (var newStat in state.StatementStatistics[newStatement.StatementFingerprint])
                        {
                            newStat.NormalizedStatementID = newStatement.ID;
                            PersistStatementStatistics(newStat);
                        }
                    }
                    if (state.StatementIndexStatistics.ContainsKey(newStatement.StatementFingerprint))
                    {
                        foreach (var newStat in state.StatementIndexStatistics[newStatement.StatementFingerprint])
                        {
                            newStat.NormalizedStatementID = newStatement.ID;
                            PersistStatementIndexStatistics(newStat);
                        }
                    }
                    if (state.StatementRelationStatistics.ContainsKey(newStatement.StatementFingerprint))
                    {
                        foreach (var newStat in state.StatementRelationStatistics[newStatement.StatementFingerprint])
                        {
                            newStat.NormalizedStatementID = newStatement.ID;
                            PersistStatementRelationStatistics(newStat);
                        }
                    }
                }
                processedLogEntryEvidence.PersistCurrentState();
                scope.Complete();
            }
            statementDataAccumulator.ClearState();
        }

        private void PersistStatementIfNeeded(NormalizedStatement oldStatement, NormalizedStatement newStatement)
        {
            var repository = repositories.GetNormalizedStatementsRepository();
            if (oldStatement == null)
            {
                repository.Create(newStatement);
            }
            else
            {
                if (oldStatement.CommandType == null && newStatement.CommandType != null)
                {
                    oldStatement.CommandType = newStatement.CommandType;
                    oldStatement.StatementDefinition = newStatement.StatementDefinition;
                    repository.Update(oldStatement);
                    newStatement.ID = oldStatement.ID;
                }
            }
        }

        private void PersistStatementStatistics(NormalizedStatementStatistics newStatistics)
        {
            var repository = repositories.GetNormalizedStatementStatisticsRepository();
            var uniqueKey = new NormalizedStatementStatisticsUniqueKey()
            {
                ApplicationName = newStatistics.ApplicationName,
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                NormalizedStatementID = newStatistics.NormalizedStatementID,
                UserName = newStatistics.UserName
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                NormalizedStatementStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }

        private void PersistStatementIndexStatistics(NormalizedStatementIndexStatistics newStatistics)
        {
            var repository = repositories.GetNormalizedStatementIndexStatisticsRepository();
            var uniqueKey = new NormalizedStatementIndexStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                IndexID = newStatistics.IndexID,
                NormalizedStatementID = newStatistics.NormalizedStatementID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                NormalizedStatementIndexStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }

        private void PersistStatementRelationStatistics(NormalizedStatementRelationStatistics newStatistics)
        {
            var repository = repositories.GetNormalizedStatementRelationStatisticsRepository();
            var uniqueKey = new NormalizedStatementRelationStatisticsUniqueKey()
            {
                DatabaseID = newStatistics.DatabaseID,
                Date = newStatistics.Date,
                RelationID = newStatistics.RelationID,
                NormalizedStatementID = newStatistics.NormalizedStatementID
            };
            var oldStatistics = repository.GetByUniqueKey(uniqueKey);
            if (oldStatistics == null)
            {
                repository.Create(newStatistics);
            }
            else
            {
                NormalizedStatementRelationStatisticsMergeUtility.ApplySample(oldStatistics, newStatistics);
                repository.Update(oldStatistics);
            }
        }
    }
}
