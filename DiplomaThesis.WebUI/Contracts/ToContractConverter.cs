using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                itemToAdd.IndependentValue = s.Statement;
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
                itemToAdd.IndependentValue = s.Statement;
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
