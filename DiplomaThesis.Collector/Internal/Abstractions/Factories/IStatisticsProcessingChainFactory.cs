using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IStatisticsProcessingChainFactory
    {
        IExecutableCommand TotalStatisticsCollectNextSampleChain(TotalStatisticsCollectNextSampleContext context);
        IExecutableCommand TotalStatisticsPersistenceChain();
    }
}
