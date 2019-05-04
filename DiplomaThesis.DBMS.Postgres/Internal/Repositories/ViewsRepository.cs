﻿using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class ViewsRepository : BaseRepository, IViewsRepository
    {
        public ViewsRepository(string connectionString) : base(connectionString)
        {

        }
        public IEnumerable<IView> GetAllNonSystemViews()
        {
            var sql = @"
select d.oid as db_id, n.oid schema_id, v.schemaname schema_name, c.oid view_id, v.viewname view_name, v.definition view_definition from pg_views v
inner join pg_database d on d.datname = current_database()
inner join pg_namespace n on n.nspname = v.schemaname 
inner join pg_class c on c.relname = v.viewname and c.relnamespace = n.oid
where v.schemaname NOT IN('information_schema', 'pg_catalog')
";
            return ExecuteQuery<View>(sql, null);
        }
    }
}