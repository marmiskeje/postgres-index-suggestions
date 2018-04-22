using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class IndicesRepository : BaseRepository, IIndicesRepository
    {
        public IndicesRepository(string connectionString) : base(connectionString)
        {
            
        }

        public IEnumerable<IIndex> GetAll()
        {
            string query = @"
                SELECT d.oid as db_id, d.datname as db_name, s.oid as schema_id, s.nspname as schema_name, rr.oid as relation_id, rr.relname as relation_name, i.indexrelid as index_id, ii.relname as index_name,
                (SELECT array_to_string(array_agg(a.attname), ',') as index_attributes FROM pg_attribute a WHERE a.attrelid = i.indexrelid)
                from information_schema.tables r
                INNER JOIN pg_database d ON d.datname = r.table_catalog
                INNER JOIN pg_namespace s ON s.nspname = r.table_schema
                INNER JOIN pg_class rr ON rr.relname = r.table_name AND rr.relnamespace = s.oid
                INNER JOIN pg_index i ON i.indrelid = rr.oid
                INNER JOIN pg_class ii ON ii.oid = i.indexrelid
            ";
            return ExecuteQuery<Index>(query, null).ToList();
        }
    }
}
