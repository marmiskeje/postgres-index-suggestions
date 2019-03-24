using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class DatabasesRepository : BaseRepository, IDatabasesRepository
    {
        private const string ALL_CACHE_KEY = "ALL";
        private const string GET_BY_NAME_CACHE_FORMAT = "GET_BY_NAME_{0}";
        public DatabasesRepository(string connectionString) : base(connectionString)
        {

        }
        public IDatabase Get(uint databaseID)
        {
            string attributeKey = $"{databaseID}";
            IDatabase result = null;
            Dictionary<string, Database> all = GetAll();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        public IDatabase GetByName(string databaseName)
        {
            var cacheKey = CreateCacheKeyForThisType(String.Format(GET_BY_NAME_CACHE_FORMAT, databaseName));
            IDatabase result = null;
            if (!Cache.TryGetValue(cacheKey, out result))
            {
                result = GetAll().Where(x => x.Value.Name == databaseName).Select(x => x.Value).FirstOrDefault();
                if (result != null)
                {
                    Cache.Save(cacheKey, result, TimeSpan.FromMinutes(10)); 
                }
            }
            return result;
        }

        private Dictionary<string, Database> GetAll()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_CACHE_KEY);
            Dictionary<string, Database> all = null;
            string query = @"
                SELECT d.oid as db_id, d.datname as db_name
                FROM pg_database d
            ";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<Database>(query, null).ToDictionary(x => $"{x.ID}");
                Cache.Save(cacheKey, all, TimeSpan.FromMinutes(10));
            }
            return all;
        }
    }
}
