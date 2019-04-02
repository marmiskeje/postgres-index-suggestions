using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class TotalDatabaseStatisticsRepository : BaseRepository, ITotalDatabaseStatisticsRepository
    {
        public TotalDatabaseStatisticsRepository(string connectionString) : base(connectionString)
        {

        }
        public ITotalDatabaseStatistics GetLatest()
        {
            TotalDatabaseStatistics result = new TotalDatabaseStatistics();
            string sql = @"
select NOW() as collected_date;
select datid as db_id, stats_reset as db_stats_reset from pg_stat_database where datname = current_database();

select d.oid as db_id, r.relid as relation_id, r.* from pg_stat_user_tables r
inner join pg_database d on d.datname = current_database();

select d.oid as db_id, i.indexrelid as index_id, i.relid as relation_id, i.* from pg_stat_user_indexes i
inner join pg_database d on d.datname = current_database();

select d.oid as db_id, f.funcid as proc_id, f.* from pg_stat_user_functions f
inner join pg_database d on d.datname = current_database();
";
            ExecuteQueryMulti(sql, null, reader =>
            {
                result.CollectedDate = reader.ReadSingle<TotalDatabaseCollectionInfo>().CollectedDate;
                result.Database = reader.ReadSingle<DatabaseStatistics>();
                result.Relations = reader.Read<RelationStatistics>();
                result.Indices = reader.Read<IndexStatistics>();
                result.StoredProcedures = reader.Read<StoredProcedureStatistics>();
            });
            return result;
        }
    }
}
