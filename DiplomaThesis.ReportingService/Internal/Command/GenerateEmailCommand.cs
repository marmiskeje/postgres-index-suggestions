using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.ReportingService
{
    internal class GenerateEmailCommand<TModel> : ChainableCommand
    {
        private readonly ReportContextWithModel<TModel> context;
        private readonly ISettingPropertiesRepository settingPropertiesRepository;
        private readonly IRazorEngine razorEngine;
        public GenerateEmailCommand(ReportContextWithModel<TModel> context, ISettingPropertiesRepository settingPropertiesRepository, IRazorEngine razorEngine)
        {
            this.context = context;
            this.settingPropertiesRepository = settingPropertiesRepository;
            this.razorEngine = razorEngine;
        }

        protected override void OnExecute()
        {
            var reportingSettings = settingPropertiesRepository.GetObject<ReportingSettings>(SettingPropertyKeys.REPORTING_SETTINGS);
            var template = settingPropertiesRepository.GetObject<EmailTemplate>(context.TemplateId);
            if (reportingSettings != null && reportingSettings.Recipients.Count > 0 && template != null)
            {
                context.EmailDefinition = new EmailDefinition()
                {
                    Body = razorEngine.Transform(template.BodyTemplate, context.Model),
                    Recipients = reportingSettings.Recipients,
                    IsBodyHtml = template.IsBodyHtml,
                    Subject = template.Subject
                };
            }
            else
            {
                IsEnabledSuccessorCall = false;
            }
        }
    }
}