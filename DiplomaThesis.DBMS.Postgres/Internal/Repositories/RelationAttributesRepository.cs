using DiplomaThesis.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class RelationAttributesRepository : BaseRepository, IRelationAttributesRepository
    {
        private const string ALL_BY_RELATION_ATTRIBUTE_NUMBER_CACHE_KEY = "ALL_BY_RELATION_ATTRIBUTE_NUMBER";
        private const string ALL_BY_RELATION_ATTRIBUTE_NAME_CACHE_KEY = "ALL_BY_RELATION_ATTRIBUTE_NAME";
        public RelationAttributesRepository(string connectionString) : base(connectionString)
        {
            
        }
        public IRelationAttribute Get(uint relationID, int attributeNumber)
        {
            string attributeKey = $"{relationID}_{attributeNumber}";
            IRelationAttribute result = null;
            Dictionary<string, RelationAttribute> all = GetAllByRelationIdAttributeNumber();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        public IRelationAttribute Get(uint relationID, string attributeName)
        {
            string attributeKey = $"{relationID}_{attributeName}";
            IRelationAttribute result = null;
            Dictionary<string, RelationAttribute> all = GetAllByRelationIdAttributeName();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        private Dictionary<string, RelationAttribute> GetAllByRelationIdAttributeNumber()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_BY_RELATION_ATTRIBUTE_NUMBER_CACHE_KEY);
            Dictionary<string, RelationAttribute> all = null;
            string query = @"
select a.attrelid attr_rel_id, a.attname attr_name, a.atttypid attr_typeid, a.attnum attr_number,
(select string_agg(oprname, ',') from (select distinct o.oprname from pg_catalog.pg_operator o where o.oprleft = a.atttypid or o.oprright = a.atttypid) t) attr_operators,
(case when stats.n_distinct > 0 and rel.reltuples > 0 then rel.reltuples / stats.n_distinct
else stats.n_distinct end) attr_stats_n_distinct,
(select string_agg(val, '¤') from (select unnest(stats.most_common_vals::text::text[]) val) mcv) attr_stats_most_common_vals,
stats.most_common_freqs attr_stats_most_common_freqs,
 (select string_agg(val, '¤') from (select unnest(stats.histogram_bounds::text::text[]) val) hb) attr_stats_histogram_bounds,
(not a.attnotnull) attr_is_nullable
from pg_catalog.pg_attribute a
inner join pg_class rel on rel.oid = a.attrelid
inner join pg_namespace n on n.oid = rel.relnamespace 
left outer join pg_stats stats on stats.schemaname = n.nspname and stats.tablename = rel.relname and stats.attname = a.attname
";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<RelationAttribute>(query, null).ToDictionary(x => $"{x.RelationID}_{x.AttributeNumber}");
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            return all;
        }
        private Dictionary<string, RelationAttribute> GetAllByRelationIdAttributeName()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_BY_RELATION_ATTRIBUTE_NAME_CACHE_KEY);
            Dictionary<string, RelationAttribute> all = null;
            string query = @"
select a.attrelid attr_rel_id, a.attname attr_name, a.atttypid attr_typeid, a.attnum attr_number,
(select string_agg(oprname, ',') from (select distinct o.oprname from pg_catalog.pg_operator o where o.oprleft = a.atttypid or o.oprright = a.atttypid) t) attr_operators,
(case when stats.n_distinct > 0 and rel.reltuples > 0 then rel.reltuples / stats.n_distinct
else stats.n_distinct end) attr_stats_n_distinct,
(select string_agg(val, '¤') from (select unnest(stats.most_common_vals::text::text[]) val) mcv) attr_stats_most_common_vals,
stats.most_common_freqs attr_stats_most_common_freqs,
 (select string_agg(val, '¤') from (select unnest(stats.histogram_bounds::text::text[]) val) hb) attr_stats_histogram_bounds,
(not a.attnotnull) attr_is_nullable
from pg_catalog.pg_attribute a
inner join pg_class rel on rel.oid = a.attrelid
inner join pg_namespace n on n.oid = rel.relnamespace 
left outer join pg_stats stats on stats.schemaname = n.nspname and stats.tablename = rel.relname and stats.attname = a.attname
";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<RelationAttribute>(query, null).ToDictionary(x => $"{x.RelationID}_{x.Name}");
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            return all;
        }
    }
}
