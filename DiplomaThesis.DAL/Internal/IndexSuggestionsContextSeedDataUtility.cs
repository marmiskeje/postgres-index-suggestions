using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using DiplomaThesis.Common;

namespace DiplomaThesis.DAL
{
    internal static class IndexSuggestionsContextSeedDataUtility
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            uint tpccDaabaseID = 371580;
            uint testDatabaseID = 16393;
            //workloads
            var tpccWorkload = new Workload()
            {
                ID = 1,
                Name = "TPCC",
                DatabaseID = tpccDaabaseID,
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
            tpccWorkload.Definition.Relations.ForbiddenValues.AddRange(new uint[] { 2619, 3118, 1260, 1418, 6100, 2613, 1247, 1249, 1255, 1259, 2604, 2606, 2611, 2610, 2617, 2753, 2616, 2601, 2602, 2603, 2612, 2995, 2600, 3381, 2618, 2620, 3466, 2609, 2605, 3501, 2615, 2607, 2608, 1262, 2964, 1213, 1136, 1261, 1214, 2396, 3602, 3603, 3600, 3601, 3764, 3079, 2328, 1417, 3256, 6000, 826, 3394, 3596, 3592, 3456, 3350, 3541, 3576, 2224, 6104, 6106, 6102, 12847, 12852, 12857, 12862, 12867, 12872, 12877 });
            tpccWorkload.DefinitionData = SerializeToJson(tpccWorkload.Definition);
            

            var testWorkload = new Workload()
            {
                ID = 2,
                Name = "MM test",
                DatabaseID = testDatabaseID,
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
            testWorkload.Definition.Relations.ForbiddenValues.AddRange(new uint[] { 2619, 3118, 1260, 1418, 6100, 2613, 1247, 1249, 1255, 1259, 2604, 2606, 2611, 2610, 2617, 2753, 2616, 2601, 2602, 2603, 2612, 2995, 2600, 3381, 2618, 2620, 3466, 2609, 2605, 3501, 2615, 2607, 2608, 1262, 2964, 1213, 1136, 1261, 1214, 2396, 3602, 3603, 3600, 3601, 3764, 3079, 2328, 1417, 3256, 6000, 826, 3394, 3596, 3592, 3456, 3350, 3541, 3576, 2224, 6104, 6106, 6102, 12847, 12852, 12857, 12862, 12867, 12872, 12877 });
            testWorkload.DefinitionData = SerializeToJson(testWorkload.Definition);

            modelBuilder.Entity<Workload>().HasData(tpccWorkload);
            modelBuilder.Entity<Workload>().HasData(testWorkload);
            //

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

            CollectorConfiguration collectorConfiguration = new CollectorConfiguration();
            collectorConfiguration.Databases.Add(tpccDaabaseID, new CollectorDatabaseConfiguration() { DatabaseID = tpccDaabaseID, IsEnabledGeneralCollection = true, IsEnabledStatementCollection = true });
            collectorConfiguration.Databases.Add(testDatabaseID, new CollectorDatabaseConfiguration() { DatabaseID = testDatabaseID, IsEnabledGeneralCollection = true, IsEnabledStatementCollection = true });
            modelBuilder.Entity<SettingProperty>().HasData(new SettingProperty() { ID = 5, Key = SettingPropertyKeys.COLLECTOR_CONFIGURATION, StrValue = SerializeToJson(collectorConfiguration) });
        }

        private static string SerializeToJson<T>(T data)
        {
            return JsonSerializationUtility.Serialize(data);
        }
    }
}
