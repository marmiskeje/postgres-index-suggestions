using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class StoredProceduresRepository : BaseRepository, IStoredProceduresRepository
    {
        public StoredProceduresRepository(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<IStoredProcedure> GetAll()
        {
            var sql = @"
select d.oid db_id, p.oid proc_id, p.proname proc_name, n.oid schema_id, n.nspname schema_name, p.pronargs proc_args_count, p.prosrc proc_definition,
l.lanname lang_name
from pg_proc p
inner join pg_database d on d.datname = current_database()
inner join pg_namespace n on n.oid = p.pronamespace
inner join pg_language l on l.oid = p.prolang";
            return ExecuteQuery<StoredProcedure>(sql, null);
        }

        public IEnumerable<IStoredProcedure> GetAllNonSystems()
        {
            var sql = @"
select d.oid db_id, p.oid proc_id, p.proname proc_name, n.oid schema_id, n.nspname schema_name, p.pronargs proc_args_count, p.prosrc proc_definition,
l.lanname lang_name
from pg_proc p
inner join pg_database d on d.datname = current_database()
inner join pg_namespace n on n.oid = p.pronamespace
inner join pg_language l on l.oid = p.prolang
where n.nspname NOT IN (" + String.Join(", ", SystemObjects.SystemSchemas.Select(x => "'" + x + "'")) + ")";
            return ExecuteQuery<StoredProcedure>(sql, null);
        }
    }
}
