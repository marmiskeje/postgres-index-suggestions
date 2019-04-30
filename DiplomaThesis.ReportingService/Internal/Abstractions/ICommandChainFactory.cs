using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.ReportingService
{
    internal interface ICommandChainFactory
    {
        IExecutableCommand SummaryReportChain(ReportContextWithModel<SummaryEmailModel> context);
    }
}
