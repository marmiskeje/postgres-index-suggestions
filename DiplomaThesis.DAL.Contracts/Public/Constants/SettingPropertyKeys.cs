using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public static class SettingPropertyKeys
    {
        public const string LAST_PROCESSED_LOG_ENTRY_TIMESTAMP = "LastProcessedLogEntryTimestamp";
        public const string SMTP_CONFIGURATION = "SmtpConfiguration";
        public const string EMAIL_TEMPLATE_SUMMARY_REPORT = "EmailTemplateSummaryReport";
        public const string REPORTING_SETTINGS = "ReportingSettings";
        public const string COLLECTOR_CONFIGURATION = "CollectorConfiguration";
    }
}
