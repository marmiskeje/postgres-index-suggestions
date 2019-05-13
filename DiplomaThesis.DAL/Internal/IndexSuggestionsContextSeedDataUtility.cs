using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal static class IndexSuggestionsContextSeedDataUtility
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            //workloads
            var workload = new Workload()
            {
                ID = 1,
                Name = "TestWorkload",
                DatabaseID = 1,
                CreatedDate = DateTime.Now,
                Definition = new WorkloadDefinition()
                {
                    Applications = new WorkloadPropertyValuesDefinition<string>(),
                    DateTimeSlots = new WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot>(),
                    QueryThresholds = new WorkloadQueryThresholds(),
                    Relations = new WorkloadPropertyValuesDefinition<uint>(),
                    Users = new WorkloadPropertyValuesDefinition<string>()
                }
            };
            workload.DefinitionData = SerializeToJson(workload.Definition);
            modelBuilder.Entity<Workload>().HasData(workload);

            // settings
            modelBuilder.Entity<SettingProperty>().HasData(new SettingProperty() { ID = 1, Key = SettingPropertyKeys.LAST_PROCESSED_LOG_ENTRY_TIMESTAMP });

            ReportingSettings reportingSettings = new ReportingSettings();
            reportingSettings.Recipients.Add("mm.diploma.thesis@gmail.com");
            modelBuilder.Entity<SettingProperty>().HasData(new SettingProperty() { ID = 2, Key = SettingPropertyKeys.REPORTING_SETTINGS, StrValue = SerializeToJson(reportingSettings)});

            SmtpConfiguration smtpConfiguration = new SmtpConfiguration();
            smtpConfiguration.SmtpHost = "smtp.gmail.com";
            smtpConfiguration.SmtpPasswordCipher = Common.Cryptography.EncryptionSupport.Instance.Encrypt("mm.d1pl0ma.thes1s.p$$s");
            smtpConfiguration.SmtpPort = 587;
            smtpConfiguration.SmtpUsername = "mm.diploma.thesis@gmail.com";
            smtpConfiguration.SystemEmailSender = "mm.diploma.thesis@gmail.com";
            modelBuilder.Entity<SettingProperty>().HasData(new SettingProperty() { ID = 3, Key = SettingPropertyKeys.SMTP_CONFIGURATION, StrValue = SerializeToJson(smtpConfiguration) });

            EmailTemplate summaryReportTemplate = new EmailTemplate();
            summaryReportTemplate.IsBodyHtml = true;
            summaryReportTemplate.Subject = "DBMS statistics - summary report";
            summaryReportTemplate.BodyTemplate = Internal.DefaultEmailTemplates.SummaryReport;
            modelBuilder.Entity<SettingProperty>().HasData(new SettingProperty() { ID = 4, Key = SettingPropertyKeys.EMAIL_TEMPLATE_SUMMARY_REPORT, StrValue = SerializeToJson(summaryReportTemplate) });
        }

        private static string SerializeToJson<T>(T data)
        {
            return JsonSerializationUtility.Serialize(data);
        }
    }
}
