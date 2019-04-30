using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;

namespace DiplomaThesis.ReportingService
{
    internal interface ICommandFactory
    {
        IChainableCommand SendEmailCommand(ReportContext context);
        IChainableCommand LoadDataAndCreateEmailModelCommand(ReportContextWithModel<SummaryEmailModel> context);
        IChainableCommand GenerateEmailCommand<TModel>(ReportContextWithModel<TModel> context);
    }
}
