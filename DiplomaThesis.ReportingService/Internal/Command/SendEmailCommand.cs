using System;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System.Net.Mail;
using System.Net;
using System.Text;
using DiplomaThesis.Common.Logging;

namespace DiplomaThesis.ReportingService
{
    internal class SendEmailCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly ReportContext context;
        private readonly ISettingPropertiesRepository settingPropertiesRepository;

        public SendEmailCommand(ILog log, ReportContext context, ISettingPropertiesRepository settingPropertiesRepository)
        {
            this.log = log;
            this.context = context;
            this.settingPropertiesRepository = settingPropertiesRepository;
        }

        protected override void OnExecute()
        {
            var configuration = settingPropertiesRepository.GetObject<SmtpConfiguration>(SettingPropertyKeys.SMTP_CONFIGURATION);
            if (configuration != null)
            {
                using (SmtpClient client = new SmtpClient(configuration.SmtpHost, configuration.SmtpPort))
                {
                    client.EnableSsl = true;
                    if (!String.IsNullOrEmpty(configuration.SmtpUsername) && !String.IsNullOrEmpty(configuration.SmtpPassword))
                    {
                        client.Credentials = new NetworkCredential(configuration.SmtpUsername, configuration.SmtpPassword);
                    }
                    else
                    {
                        client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    }
                    using (var message = ConvertMessage(context.EmailDefinition, configuration.SystemEmailSender))
                    {
                        client.Send(message);
                        log.Write(SeverityType.Info, "Email with subject: {0} sent. (recipients: {1})",
                                        context.EmailDefinition.Subject, string.Join(",", context.EmailDefinition.Recipients));
                    }
                } 
            }
        }

        private MailMessage ConvertMessage(EmailDefinition email, string systemSender)
        {
            MailMessage result = new MailMessage();
            result.From = new MailAddress(email.Sender ?? systemSender);
            foreach (string r in email.Recipients)
            {
                result.To.Add(new MailAddress(r));
            }
            result.IsBodyHtml = email.IsBodyHtml;
            result.BodyEncoding = Encoding.UTF8;
            result.Subject = email.Subject;
            result.SubjectEncoding = Encoding.UTF8;
            result.Body = email.Body;
            return result;
        }
    }
}