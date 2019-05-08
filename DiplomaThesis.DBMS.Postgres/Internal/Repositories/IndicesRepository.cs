using DiplomaThesis.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace DiplomaThesis.DBMS.Postgres
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
                (SELECT array_to_string(array_agg(t.attname), ',') as index_attributes FROM (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = i.indexrelid ORDER BY a.attnum) t),
				(case
				 when ia.amname = 'btree' then 1
				 when ia.amname = 'hash' then 2
				 when ia.amname = 'gist' then 3
				 when ia.amname = 'gin' then 4
				 when ia.amname = 'spgist' then 5
				 when ia.amname = 'brin' then 6
				 else 0
				end) index_access_method,
				iii.indexdef index_definition
                FROM information_schema.tables r
                INNER JOIN pg_database d ON d.datname = r.table_catalog
                INNER JOIN pg_namespace s ON s.nspname = r.table_schema
                INNER JOIN pg_class rr ON rr.relname = r.table_name AND rr.relnamespace = s.oid
                INNER JOIN pg_index i ON i.indrelid = rr.oid
                INNER JOIN pg_class ii ON ii.oid = i.indexrelid
				INNER JOIN pg_am ia ON ia.oid = ii.relam
				INNER JOIN pg_indexes iii ON iii.schemaname = s.nspname AND iii.tablename = rr.relname AND iii.indexname = ii.relname

            ";
            return ExecuteQuery<Index>(query, null);
        }

        public IEnumerable<IIndex> GetAllNonSystems()
        {
            string query = @"
                SELECT d.oid as db_id, d.datname as db_name, s.oid as schema_id, s.nspname as schema_name, rr.oid as relation_id, rr.relname as relation_name, i.indexrelid as index_id, ii.relname as index_name,
                (SELECT array_to_string(array_agg(t.attname), ',') as index_attributes FROM (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = i.indexrelid ORDER BY a.attnum) t),
				(case
				 when ia.amname = 'btree' then 1
				 when ia.amname = 'hash' then 2
				 when ia.amname = 'gist' then 3
				 when ia.amname = 'gin' then 4
				 when ia.amname = 'spgist' then 5
				 when ia.amname = 'brin' then 6
				 else 0
				end) index_access_method,
				iii.indexdef index_definition
                FROM information_schema.tables r
                INNER JOIN pg_database d ON d.datname = r.table_catalog
                INNER JOIN pg_namespace s ON s.nspname = r.table_schema
                INNER JOIN pg_class rr ON rr.relname = r.table_name AND rr.relnamespace = s.oid
                INNER JOIN pg_index i ON i.indrelid = rr.oid
                INNER JOIN pg_class ii ON ii.oid = i.indexrelid
				INNER JOIN pg_am ia ON ia.oid = ii.relam
				INNER JOIN pg_indexes iii ON iii.schemaname = s.nspname AND iii.tablename = rr.relname AND iii.indexname = ii.relname
                WHERE s.nspname NOT IN (" + String.Join(", ", SystemObjects.SystemSchemas.Select(x => "'" + x + "'")) + ")";
            return ExecuteQuery<Index>(query, null);
        }
    }
}
