using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.Collector
{
    internal class GeneralProcessingCommandFactory : IGeneralProcessingCommandFactory
    {
        private readonly ILog log;
        private readonly ICollectorConfiguration configuration;
        private readonly IStatementsProcessingDataAccumulator statementDataAccumulator;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly ILastProcessedLogEntryEvidence processedLogEntryEvidence;
        public GeneralProcessingCommandFactory(ILog log, ICollectorConfiguration configuration, IStatementsProcessingDataAccumulator statementDataAccumulator,
                                               DBMS.Contracts.IRepositoriesFactory dbmsRepositories, IRepositoriesFactory dalRepositories,
                                               ILastProcessedLogEntryEvidence processedLogEntryEvidence)
        {
            this.log = log;
            this.configuration = configuration;
            this.statementDataAccumulator = statementDataAccumulator;
            this.dbmsRepositories = dbmsRepositories;
            this.dalRepositories = dalRepositories;
            this.processedLogEntryEvidence = processedLogEntryEvidence;
        }

        public IChainableCommand ComputeNormalizedStatementFingerprintCommand(LogEntryProcessingContext context)
        {
            return new ComputeNormalizedStatementFingerprintCommand(context);
        }

        public IChainableCommand ExternalStatementNormalizationCommand(LogEntryProcessingContext context)
        {
            return new ExternalStatementNormalizationCommand(log, configuration.ExternalSqlNormalization, context);
        }

        public IChainableCommand HandleExceptionCommand(Action<Exception> onExceptionAction = null, Action finallyAction = null)
        {
            return new HandleExceptionCommand(onExceptionAction, finallyAction);
        }

        public IChainableCommand LoadDatabaseInfoForLogEntryCommand(LogEntryProcessingContext context)
        {
            return new LoadDatabaseInfoForLogEntryCommand(log, context, dbmsRepositories.GetDatabasesRepository());
        }

        public IChainableCommand PersistDataAccumulatorsCommand()
        {
            return new PersistLogEntryDataAccumulatorsCommand(statementDataAccumulator, dalRepositories, processedLogEntryEvidence);
        }

        public IChainableCommand PublishNormalizedStatementCommand(LogEntryProcessingContext context)
        {
            return new PublishNormalizedStatementCommand(context, statementDataAccumulator);
        }

        public IChainableCommand PublishNormalizedStatementStatisticsCommand(LogEntryProcessingContext context)
        {
            return new PublishNormalizedStatementStatisticsCommand(context, statementDataAccumulator);
        }

        public IChainableCommand PublishNormalizedStatementDefinitionCommand(LogEntryProcessingContext context)
        {
            return new PublishNormalizedStatementDefinitionCommand(context, statementDataAccumulator);
        }

        public IChainableCommand PublishNormalizedStatementIndexStatisticsCommand(LogEntryProcessingContext context)
        {
            return new PublishNormalizedStatementIndexStatisticsCommand(context, statementDataAccumulator);
        }

        public IChainableCommand PublishNormalizedStatementRelationStatisticsCommand(LogEntryProcessingContext context)
        {
            return new PublishNormalizedStatementRelationStatisticsCommand(context, statementDataAccumulator);
        }
    }
}
