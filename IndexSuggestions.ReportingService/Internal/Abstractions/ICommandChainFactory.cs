using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.ReportingService
{
    internal interface ICommandChainFactory
    {
        IExecutableCommand SummaryReportChain(ReportContextWithModel<SummaryEmailModel> context);
    }
}
