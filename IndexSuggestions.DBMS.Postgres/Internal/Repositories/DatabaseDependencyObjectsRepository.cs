using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class DatabaseDependencyObjectsRepository : BaseRepository, IDatabaseDependencyObjectsRepository
    {
        public DatabaseDependencyObjectsRepository(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<IDatabaseDependencyObject> GetDependenciesForDatabaseObject(uint id)
        {
            var sql = @"
SELECT distinct r.ev_class::regclass AS source, dependend.refobjid::regclass AS dependency,
d.oid db_id, dependency.oid view_id, dependency.relname view_name, dependency_nsp.oid schema_id, dependency_nsp.nspname schema_name, v.definition view_definition
FROM   pg_rewrite r
inner join pg_depend dependend ON dependend.objid = r.oid
inner join pg_class source on source.oid = r.ev_class
inner join pg_class dependency on dependency.oid = dependend.refobjid and dependency.relkind = 'v'
inner join pg_namespace dependency_nsp on dependency_nsp.oid = dependency.relnamespace
inner join pg_database d on d.datname = current_database()
inner join pg_views v on v.viewname = dependency.relname and v.schemaname = dependency_nsp.nspname
where r.ev_class = @ObjectId
and (r.ev_class != dependency.oid);
SELECT distinct r.ev_class::regclass AS source, dependend.refobjid::regprocedure AS dependency,
d.oid db_id, dependency.oid proc_id, dependency.proname proc_name, dependency_nsp.oid schema_id, dependency_nsp.nspname schema_name, dependency.pronargs proc_args_count, dependency.prosrc proc_definition,
l.lanname lang_name
FROM   pg_rewrite r
inner join pg_depend dependend ON dependend.objid = r.oid and dependend.refclassid = 'pg_proc'::regclass
inner join pg_class source on source.oid = r.ev_class
inner join pg_proc dependency on dependency.oid = dependend.refobjid
inner join pg_namespace dependency_nsp on dependency_nsp.oid = dependency.pronamespace
inner join pg_database d on d.datname = current_database()
inner join pg_language l on l.oid = dependency.prolang 
where r.ev_class = @ObjectId
and (r.ev_class != dependency.oid)
";
            List<IDatabaseDependencyObject> result = new List<IDatabaseDependencyObject>();
            ExecuteQueryMulti(sql, new { ObjectId = (long)id }, reader =>
            {
                result.AddRange(reader.Read<View>());
                result.AddRange(reader.Read<StoredProcedure>());
            });
            return result;
        }
    }
}
