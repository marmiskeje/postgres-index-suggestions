using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IGeneralProcessingCommandFactory
    {
        IChainableCommand HandleExceptionCommand(Action<Exception> onExceptionAction = null, Action finallyAction = null);
        IChainableCommand ExternalStatementNormalizationCommand(LogEntryProcessingContext context);
        IChainableCommand ComputeNormalizedStatementFingerprintCommand(LogEntryProcessingContext context);
        IChainableCommand PublishNormalizedStatementCommand(LogEntryProcessingContext context);
        IChainableCommand LoadDatabaseInfoForLogEntryCommand(LogEntryProcessingContext context);
        IChainableCommand PublishNormalizedStatementStatisticsCommand(LogEntryProcessingContext context);
        IChainableCommand PersistDataAccumulatorsCommand();
        IChainableCommand PublishNormalizedStatementDefinitionCommand(LogEntryProcessingContext context);
        IChainableCommand PublishNormalizedStatementIndexStatisticsCommand(LogEntryProcessingContext context);
        IChainableCommand PublishNormalizedStatementRelationStatisticsCommand(LogEntryProcessingContext context);
    }
}
