using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class ViewsRepository : BaseRepository, IViewsRepository
    {
        public ViewsRepository(string connectionString) : base(connectionString)
        {

        }
        public IEnumerable<IView> GetAll()
        {
            var sql = @"
select d.oid as db_id, n.oid schema_id, v.schemaname schema_name, c.oid view_id, v.viewname view_name, v.definition view_definition from pg_views v
inner join pg_database d on d.datname = current_database()
inner join pg_namespace n on n.nspname = v.schemaname 
inner join pg_class c on c.relname = v.viewname and c.relnamespace = n.oid
";
            return ExecuteQuery<View>(sql, null);
        }
    }
}
