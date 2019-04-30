using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.ReportingService
{
    internal class CommandFactory : ICommandFactory
    {
        private readonly ILog log;
        private readonly IRepositoriesFactory dalRepositories;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        private readonly IRazorEngine razorEngine;
        public CommandFactory(ILog log, IRepositoriesFactory dalRepositories, DBMS.Contracts.IRepositoriesFactory dbmsRepositories, IRazorEngine razorEngine)
        {
            this.log = log;
            this.dalRepositories = dalRepositories;
            this.dbmsRepositories = dbmsRepositories;
            this.razorEngine = razorEngine;
        }

        public IChainableCommand GenerateEmailCommand<TModel>(ReportContextWithModel<TModel> context)
        {
            return new GenerateEmailCommand<TModel>(context, dalRepositories.GetSettingPropertiesRepository(), razorEngine);
        }

        public IChainableCommand LoadDataAndCreateEmailModelCommand(ReportContextWithModel<SummaryEmailModel> context)
        {
            return new LoadDataAndCreateEmailModelCommand(log, context, dbmsRepositories, dalRepositories);
        }

        public IChainableCommand SendEmailCommand(ReportContext context)
        {
            return new SendEmailCommand(log, context, dalRepositories.GetSettingPropertiesRepository());
        }
    }
}
