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
            string query = "SELECT attrelid, attname, atttypid, attnum FROM pg_catalog.pg_attribute";
            return ExecuteQuery<RelationAttribute>(query, null).ToDictionary(x => $"{x.RelationID}_{x.AttributeNumber}");
        }
    }
}
