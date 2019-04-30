using DiplomaThesis.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class RelationsRepository : BaseRepository, IRelationsRepository
    {
        private const string ALL_CACHE_KEY = "ALL";
        public RelationsRepository(string connectionString) : base(connectionString)
        {
            
        }

        public IRelation Get(uint relationID)
        {
            string attributeKey = $"{relationID}";
            var cacheKey = CreateCacheKeyForThisType(ALL_CACHE_KEY);
            Dictionary<string, Relation> all = null;
            IRelation result = null;
            if (!Cache.TryGetValue(cacheKey, out all) || !all.ContainsKey(attributeKey))
            {
                all = GetAll();
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }
#warning pozor, toto je iba per aktualna databaza, treba prerobit, aj na ostatnych miestach
        private Dictionary<string, Relation> GetAll()
        {
            string query = @"
select d.oid as db_id, d.datname as db_name, s.oid as schema_id, s.nspname as schema_name, rr.oid as relation_id, rr.relname as relation_name,
pg_relation_size(rr.oid) as relation_size, rr.reltuples relation_tuples_count,
(SELECT array_to_string(array_agg(t.attname), ',') as primary_key_attributes FROM (SELECT a.attname FROM pg_attribute a WHERE a.attrelid = pi.indexrelid ORDER BY a.attnum) t)
FROM information_schema.tables r
INNER JOIN pg_database d ON d.datname = r.table_catalog
INNER JOIN pg_namespace s ON s.nspname = r.table_schema
INNER JOIN pg_class rr ON rr.relname = r.table_name AND rr.relnamespace = s.oid
LEFT OUTER JOIN pg_index pi ON pi.indrelid = rr.oid AND pi.indisprimary
            ";
            return ExecuteQuery<Relation>(query, null).ToDictionary(x => $"{x.ID}");
        }
    }
}
