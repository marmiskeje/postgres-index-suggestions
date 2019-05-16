using DiplomaThesis.DAL.Contracts;
using System;
using DiplomaThesis.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiplomaThesis.WebUI
{
    public partial class ContractConverter
    {
        public Workload CreateWorkload(WorkloadData source)
        {
            Workload result = new Workload();
            result.CreatedDate = DateTime.Now;
            result.DatabaseID = source.DatabaseID;
            result.Definition = new WorkloadDefinition();
            result.Definition.Applications = new WorkloadPropertyValuesDefinition<string>();
            result.Definition.DateTimeSlots = new WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot>();
            result.Definition.QueryThresholds = new WorkloadQueryThresholds();
            result.Definition.Relations = new WorkloadPropertyValuesDefinition<uint>();
            result.Definition.Users = new WorkloadPropertyValuesDefinition<string>();
            if (source.Definition != null)
            {
                if (source.Definition.ForbiddenApplications != null)
                {
                    result.Definition.Applications.ForbiddenValues.AddRange(source.Definition.ForbiddenApplications);
                }
                if (source.Definition.ForbiddenDateTimeSlots != null)
                {
                    foreach (var slot in source.Definition.ForbiddenDateTimeSlots)
                    {
                        result.Definition.DateTimeSlots.ForbiddenValues.Add(Convert(slot));
                    }
                }
                if (source.Definition.ForbiddenRelations != null)
                {
                    result.Definition.Relations.ForbiddenValues.AddRange(source.Definition.ForbiddenRelations);
                }
                if (source.Definition.ForbiddenUsers != null)
                {
                    result.Definition.Users.ForbiddenValues.AddRange(source.Definition.ForbiddenUsers);
                }
                result.Definition.QueryThresholds.MinDuration = TimeSpan.FromMilliseconds(source.Definition.StatementMinDurationInMs);
                result.Definition.QueryThresholds.MinExectutionCount = source.Definition.StatementMinExectutionCount;
            }
            result.Name = source.Name;
            return result;
        }

        internal WorkloadAnalysisData Convert(WorkloadAnalysis source)
        {
            WorkloadAnalysisData result = new WorkloadAnalysisData();
            result.CreatedDate = source.CreatedDate;
            result.EndDate = source.EndDate;
            result.ErrorMessage = source.ErrorMessage;
            result.ID = source.ID;
            result.PeriodFromDate = source.PeriodFromDate;
            result.PeriodToDate = source.PeriodToDate;
            result.StartDate = source.StartDate;
            result.State = (int)source.State;
            result.Workload = Convert(source.Workload);
            return result;
        }

        internal ConfigurationCollectorData Convert(CollectorConfiguration source)
        {
            ConfigurationCollectorData result = new ConfigurationCollectorData();
            if (source != null)
            {
                if (source.Databases != null)
                {
                    foreach (var d in source.Databases.Values)
                    {
                        result.Databases.Add(Convert(d));
                    }
                }
            }
            return result;
        }

        private ConfigurationCollectorDatabaseData Convert(CollectorDatabaseConfiguration source)
        {
            ConfigurationCollectorDatabaseData result = new ConfigurationCollectorDatabaseData();
            result.DatabaseID = source.DatabaseID;
            result.IsEnabledGeneralCollection = source.IsEnabledGeneralCollection;
            result.IsEnabledStatementCollection = source.IsEnabledStatementCollection;
            return result;
        }

        internal ConfigurationReportsData Convert(ReportingSettings source)
        {
            ConfigurationReportsData result = new ConfigurationReportsData();
            if (source != null)
            {
                result.EmailAddresses = String.Join(", ", source.Recipients);
            }
            return result;
        }

        internal ConfigurationSmtpData Convert(SmtpConfiguration source)
        {
            ConfigurationSmtpData result = new ConfigurationSmtpData();
            if (source != null)
            {
                result.Host = source.SmtpHost;
                result.Password = null;
                result.Port = source.SmtpPort;
                result.Username = source.SmtpUsername;
            }
            return result;
        }

        private WorkloadDateTimeSlot Convert(WorkloadDateTimeSlotData source)
        {
            WorkloadDateTimeSlot result = new WorkloadDateTimeSlot();
            result.DayOfWeek = source.DayOfWeek;
            result.EndTime = source.EndTime;
            result.StartTime = source.StartTime;
            return result;
        }
    }
}
