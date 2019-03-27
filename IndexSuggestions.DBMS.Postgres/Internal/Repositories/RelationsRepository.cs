using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace IndexSuggestions.DBMS.Postgres
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

        private Dictionary<string, Relation> GetAll()
        {
            string query = @"
                select d.oid as db_id, d.datname as db_name, s.oid as schema_id, s.nspname as schema_name, rr.oid as relation_id, rr.relname as relation_name
                from information_schema.tables r
                INNER JOIN pg_database d ON d.datname = r.table_catalog
                INNER JOIN pg_namespace s ON s.nspname = r.table_schema
                INNER JOIN pg_class rr ON rr.relname = r.table_name AND rr.relnamespace = s.oid
            ";
            return ExecuteQuery<Relation>(query, null).ToDictionary(x => $"{x.ID}");
        }
    }
}
