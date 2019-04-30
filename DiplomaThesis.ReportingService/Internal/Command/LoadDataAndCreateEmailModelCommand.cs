using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using DiplomaThesis.Common.Logging;

namespace DiplomaThesis.ReportingService
{
    internal class LoadDataAndCreateEmailModelCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly ReportContextWithModel<SummaryEmailModel> context;
        private readonly DBMS.Contracts.IDatabasesRepository databasesRepository;
        private readonly DBMS.Contracts.IRelationsRepository relationsRepository;
        private readonly ITotalRelationStatisticsRepository totalRelationStatisticsRepository;
        private readonly INormalizedStatementStatisticsRepository normalizedStatementStatisticsRepository;
        private readonly INormalizedStatementsRepository normalizedStatementsRepository;

        public LoadDataAndCreateEmailModelCommand(ILog log, ReportContextWithModel<SummaryEmailModel> context, DBMS.Contracts.IRepositoriesFactory dbmsRepositories,
                                                  IRepositoriesFactory dalRepositories)
        {
            this.log = log;
            this.context = context;
            databasesRepository = dbmsRepositories.GetDatabasesRepository();
            relationsRepository = dbmsRepositories.GetRelationsRepository();
            totalRelationStatisticsRepository = dalRepositories.GetTotalRelationStatisticsRepository();
            normalizedStatementStatisticsRepository = dalRepositories.GetNormalizedStatementStatisticsRepository();
            normalizedStatementsRepository = dalRepositories.GetNormalizedStatementsRepository();
        }

        protected override void OnExecute()
        {
            log.Write(SeverityType.Info, "Loading started. (templateId: {0})", context.TemplateId);
            var databases = databasesRepository.GetAll();
            log.Write(SeverityType.Info, "Loading ended. (templateId: {0})", context.TemplateId);
            var model = new SummaryEmailModel();
            model.PeriodFrom = context.DateFromInclusive;
            model.PeriodTo = context.DateToExclusive;
            foreach (var db in databases)
            {
                try
                {
                    var dbInfo = new SummaryEmailDatabaseInfo();
                    // alive relations
                    var totalStatisticsPerRelation = totalRelationStatisticsRepository.GetTotalGroupedByRelation(db.ID, context.DateFromInclusive, context.DateToExclusive);
                    var topAliveRelations = new List<SummaryEmailTopAliveRelation>();
                    foreach (var item in totalStatisticsPerRelation)
                    {
                        topAliveRelations.Add(Convert(db.Name, item.Value));
                    }
                    topAliveRelations.Sort((x, y) => y.TotalCount.CompareTo(x.TotalCount));
                    dbInfo.TopAliveRelations.AddRange(topAliveRelations.Take(10));
                    // queries
                    var totalStatisticsPerStatement = normalizedStatementStatisticsRepository.GetTotalGroupedByStatement(db.ID, context.DateFromInclusive, context.DateToExclusive);
                    var statementStatistics = new List<SummaryEmailTopStatement>();
                    foreach (var item in totalStatisticsPerStatement)
                    {
                        statementStatistics.Add(Convert(item.Value));
                    }
                    statementStatistics.Sort((x, y) => y.ExecutionCount.CompareTo(x.ExecutionCount));
                    dbInfo.TopExecutedStatements.AddRange(statementStatistics.Take(10));
                    statementStatistics.Sort((x, y) => y.MaxDuration.CompareTo(x.MaxDuration));
                    dbInfo.TopSlowestStatements.AddRange(statementStatistics.Take(10));
                    dbInfo.Name = db.Name;
                    model.Databases.Add(dbInfo);
                }
                catch (Exception ex)
                {
                    log.Write(ex);
                }
            }
            if (model.Databases.Count > 0)
            {
                context.Model = model; 
            }
            else
            {
                IsEnabledSuccessorCall = false;
            }
        }

        private SummaryEmailTopStatement Convert(NormalizedStatementStatistics source)
        {
            var result = new SummaryEmailTopStatement();
            result.AvgDuration = FormatDuration(source.AvgDuration);
            result.ExecutionCount = source.TotalExecutionsCount;
            result.MaxDuration = FormatDuration(source.MaxDuration);
            result.MinDuration = FormatDuration(source.MinDuration);
            result.Statement = normalizedStatementsRepository.GetByPrimaryKey(source.NormalizedStatementID)?.Statement ?? "Uknown";
            if (result.Statement.Length > 250)
            {
                result.Statement = result.Statement.Substring(0, 250) + "...";
            }
            result.TotalDuration = FormatDuration(source.TotalDuration);
            return result;
        }

        private string FormatDuration(TimeSpan duration)
        {
            double value = duration.TotalMilliseconds;
            string unit = "ms";
            if (duration.TotalDays >= 1)
            {
                value = duration.TotalDays;
                unit = "d";
            }
            else if (duration.TotalHours >= 1)
            {
                value = duration.TotalHours;
                unit = "h";
            }
            else if (duration.TotalMinutes >= 1)
            {
                value = duration.TotalMinutes;
                unit = "min";
            }
            else if (duration.TotalSeconds >= 1)
            {
                value = duration.TotalSeconds;
                unit = "s";
            }
            return $"{Math.Round(value, 2)} {unit}";
        }

        private SummaryEmailTopAliveRelation Convert(string dbName, TotalRelationStatistics source)
        {
            var result = new SummaryEmailTopAliveRelation();
            result.TuplesDeleteCount = source.TupleDeleteCount;
            result.IndexScansCount = source.IndexScanCount;
            result.TuplesInsertCount = source.TupleInsertCount;
            using (var scope = new DBMS.Contracts.DatabaseScope(dbName))
            {
                var relation = relationsRepository.Get(source.RelationID);
                result.Name = relation != null ? $"{relation.SchemaName}.{relation.Name}" : source.RelationID.ToString();
            }
            result.SeqScansCount = source.SeqScanCount;
            result.TuplesUpdateCount = source.TupleUpdateCount;
            result.TotalCount = source.TupleDeleteCount + source.TupleInsertCount + source.TupleUpdateCount + source.IndexScanCount + source.SeqScanCount;
            return result;
        }
    }
}