using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class RelationAttributesRepository : BaseRepository, IRelationAttributesRepository
    {
        private const string ALL_CACHE_KEY = "ALL";
        public RelationAttributesRepository(string connectionString) : base(connectionString)
        {
            
        }
        public IRelationAttribute Get(uint relationID, int attributeNumber)
        {
            string attributeKey = $"{relationID}_{attributeNumber}";
            var cacheKey = CreateCacheKeyForThisType(ALL_CACHE_KEY);
            Dictionary<string, RelationAttribute> all = null;
            IRelationAttribute result = null;
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

        private Dictionary<string, RelationAttribute> GetAll()
        {
            string query = @"
select a.attrelid attr_rel_id, a.attname attr_name, a.atttypid attr_typeid, a.attnum attr_number,
(select string_agg(oprname, ',') from (select distinct o.oprname from pg_catalog.pg_operator o where o.oprleft = a.atttypid or o.oprright = a.atttypid) t) attr_operators,
(case when stats.n_distinct > 0 and rel.reltuples > 0 then rel.reltuples / stats.n_distinct
else stats.n_distinct end) attr_stats_n_distinct,
stats.most_common_vals attr_stats_most_common_vals,
stats.most_common_freqs attr_stats_most_common_freqs
from pg_catalog.pg_attribute a
inner join pg_class rel on rel.oid = a.attrelid
inner join pg_namespace n on n.oid = rel.relnamespace 
inner join pg_stats stats on stats.schemaname = n.nspname and stats.tablename = rel.relname and stats.attname = a.attname
";
            return ExecuteQuery<RelationAttribute>(query, null).ToDictionary(x => $"{x.RelationID}_{x.AttributeNumber}");
        }
    }
}
