using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface ILogEntryProcessingChainFactory
    {
        IExecutableCommand LogEntryProcessingChain(LogEntryProcessingContext context);
        IExecutableCommand LogEntryPersistenceChain();
    }
}
