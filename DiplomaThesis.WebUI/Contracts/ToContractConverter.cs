using System;
using System.Collections.Generic;
using System.Linq;
using DiplomaThesis.Common;
using DiplomaThesis.DAL.Contracts;
using DiplomaThesis.DBMS.Contracts;
using DiplomaThesis.DBMS.Postgres;

namespace DiplomaThesis.WebUI
{
    public partial class ContractConverter
    {
        private string GetRelationName(uint databaseID, uint relationID)
        {
            using (var scope = CreateDatabaseScope(databaseID))
            {
                var relation = RepositoriesFactory.Instance.GetRelationsRepository().Get(relationID);
                return String.Format("{0}.{1}", relation?.SchemaName ?? "UnknownSchema", relation?.Name ?? "UnknownRelation");
            }
        }
        internal CollectorConfiguration CreateCollectorConfiguration(ConfigurationCollectorData source)
        {
            CollectorConfiguration result = new CollectorConfiguration();
            if (source.Databases.Count > 0)
            {
                foreach (var d in source.Databases)
                {
                    result.Databases.Add(d.DatabaseID, Convert(d));
                }
            }
            return result;
        }

        private CollectorDatabaseConfiguration Convert(ConfigurationCollectorDatabaseData source)
        {
            CollectorDatabaseConfiguration result = new CollectorDatabaseConfiguration();
            result.DatabaseID = source.DatabaseID;
            result.IsEnabledGeneralCollection = source.IsEnabledGeneralCollection;
            result.IsEnabledStatementCollection = source.IsEnabledStatementCollection;
            return result;
        }

        internal ReportingSettings CreateReportingSettings(ConfigurationReportsData source)
        {
            ReportingSettings result = new ReportingSettings();
            if (!String.IsNullOrWhiteSpace(source.EmailAddresses))
            {
                var emails = source.EmailAddresses.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                if (emails.Count() > 0)
                {
                    result.Recipients.AddRange(emails);
                }
            }
            return result;
        }

        internal SmtpConfiguration CreateSmtpConfiguration(ConfigurationSmtpData source, SmtpConfiguration original)
        {
            SmtpConfiguration result = new SmtpConfiguration();
            result.SmtpHost = source.Host;
            if (source.Password != null)
            {
                result.SmtpPasswordCipher = Common.Cryptography.EncryptionSupport.Instance.Encrypt(source.Password);
            }
            else
            {
                result.SmtpPasswordCipher = original?.SmtpPasswordCipher;
            }
            result.SmtpPort = source.Port;
            result.SmtpUsername = source.Username;
            result.SystemEmailSender = original?.SystemEmailSender;
            return result;
        }

        internal DatabaseScope CreateDatabaseScope(uint databaseID)
        {
            var dbName = RepositoriesFactory.Instance.GetDatabasesRepository().Get(databaseID)?.Name ?? String.Empty;
            return new DatabaseScope(dbName);
        }

        internal DatabaseData Convert(IDatabase source)
        {
            DatabaseData result = new DatabaseData();
            result.ID = source.ID;
            result.Name = source.Name;
            return result;
        }

        internal WorkloadData Convert(Workload source)
        {
            WorkloadData result = new WorkloadData();
            result.CreatedDate = source.CreatedDate;
            result.DatabaseID = source.DatabaseID;
            result.Definition = new WorkloadDefinitionData();
            result.Definition.ForbiddenApplications = new HashSet<string>();
            result.Definition.ForbiddenDateTimeSlots = new List<WorkloadDateTimeSlotData>();
            result.Definition.ForbiddenRelations = new HashSet<uint>();
            result.Definition.ForbiddenUsers = new HashSet<string>();
            if (source.Definition != null)
            {
                if (source.Definition.Applications != null)
                {
                    result.Definition.ForbiddenApplications.AddRange(source.Definition.Applications.ForbiddenValues);
                }
                if (source.Definition.DateTimeSlots != null)
                {
                    foreach (var slot in source.Definition.DateTimeSlots.ForbiddenValues)
                    {
                        result.Definition.ForbiddenDateTimeSlots.Add(Convert(slot));
                    }
                }
                if (source.Definition.Relations != null)
                {
                    result.Definition.ForbiddenRelations.AddRange(source.Definition.Relations.ForbiddenValues);
                }
                if (source.Definition.Users != null)
                {
                    result.Definition.ForbiddenUsers.AddRange(source.Definition.Users.ForbiddenValues);
                }
                if (source.Definition.QueryThresholds != null)
                {
                    result.Definition.StatementMinDurationInMs = System.Convert.ToInt32(source.Definition.QueryThresholds.MinDuration?.TotalMilliseconds ?? 0.0);
                    result.Definition.StatementMinExectutionCount = System.Convert.ToInt32(source.Definition.QueryThresholds.MinExectutionCount ?? 0);
                }
            }
            result.ID = source.ID;
            result.Name = source.Name;
            return result;
        }

        private WorkloadDateTimeSlotData Convert(WorkloadDateTimeSlot source)
        {
            WorkloadDateTimeSlotData result = new WorkloadDateTimeSlotData();
            result.DayOfWeek = source.DayOfWeek;
            result.EndTime = source.EndTime;
            result.StartTime = source.StartTime;
            return result;
        }

        internal List<ChartDataItem<string, long>> ConvertToMostAliveRelations(IEnumerable<SummaryTotalRelationStatistics> summaryTotalRelationStatistics)
        {
            List<ChartDataItem<string, long>> result = new List<ChartDataItem<string, long>>();
            foreach (var r in summaryTotalRelationStatistics)
            {
                var itemToAdd = new ChartDataItem<string, long>();
                itemToAdd.DependentValue = r.TotalLivenessCount;
                itemToAdd.IndependentValue = GetRelationName(r.DatabaseID, r.RelationID);
                result.Add(itemToAdd);
            }
            return result;
        }

        internal RelationData Convert(IRelation source)
        {
            RelationData result = new RelationData();
            result.ID = source.ID;
            result.Name = source.Name;
            result.SchemaName = source.SchemaName;
            return result;
        }

        internal List<ChartDataItem<string, long>> ConvertToMostExecutedStatements(IEnumerable<SummaryNormalizedStatementStatistics> topExecuted)
        {
            List<ChartDataItem<string, long>> result = new List<ChartDataItem<string, long>>();
            foreach (var s in topExecuted)
            {
                var itemToAdd = new ChartDataItem<string, long>();
                itemToAdd.DependentValue = s.TotalExecutionsCount;
                itemToAdd.IndependentValue = s.NormalizedStatement;
                result.Add(itemToAdd);
            }
            return result;
        }

        internal List<ChartDataItem<string, double>> ConvertToMostSlowestStatements(IEnumerable<SummaryNormalizedStatementStatistics> topSlowest)
        {
            List<ChartDataItem<string, double>> result = new List<ChartDataItem<string, double>>();
            foreach (var s in topSlowest)
            {
                var itemToAdd = new ChartDataItem<string, double>();
                itemToAdd.DependentValue = s.MaxDuration.TotalMilliseconds;
                itemToAdd.IndependentValue = s.NormalizedStatement;
                result.Add(itemToAdd);
            }
            return result;
        }

        internal IndexData Convert(IIndex source)
        {
            IndexData result = new IndexData();
            result.ID = source.ID;
            result.Name = source.Name;
            result.FullName = $"{source.Name} ON {source.SchemaName}.{source.RelationName}";
            return result;
        }

        internal StoredProcedureData Convert(IStoredProcedure source)
        {
            StoredProcedureData result = new StoredProcedureData();
            result.ID = source.ID;
            result.Name = source.Name;
            result.SchemaName = source.SchemaName;
            return result;
        }

        internal StatementQueryCommandType? CreateCommandTypeFilter(GetStatementsStatisticsCommandTypeFilter commandType)
        {
            switch (commandType)
            {
                case GetStatementsStatisticsCommandTypeFilter.Delete:
                    return StatementQueryCommandType.Delete;
                case GetStatementsStatisticsCommandTypeFilter.Insert:
                    return StatementQueryCommandType.Insert;
                case GetStatementsStatisticsCommandTypeFilter.Select:
                    return StatementQueryCommandType.Select;
                case GetStatementsStatisticsCommandTypeFilter.Update:
                    return StatementQueryCommandType.Update;
            }
            return null;
        }
    }
}
