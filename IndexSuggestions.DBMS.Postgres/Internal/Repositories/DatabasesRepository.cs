using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class DatabasesRepository : BaseRepository, IDatabasesRepository
    {
        private const string ALL_BY_ID_CACHE_KEY = "ALL_BY_ID";
        private const string ALL_BY_NAME_CACHE_KEY = "ALL_BY_NAME";
        public DatabasesRepository(string connectionString) : base(connectionString)
        {

        }
        public IDatabase Get(uint databaseID)
        {
            string attributeKey = $"{databaseID}";
            IDatabase result = null;
            Dictionary<string, Database> all = GetAllById();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        public IEnumerable<IDatabase> GetAll()
        {
            return GetAllById().Values;
        }

        public IDatabase GetByName(string databaseName)
        {
            string attributeKey = $"{databaseName}";
            IDatabase result = null;
            Dictionary<string, Database> all = GetAllByName();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        private Dictionary<string, Database> GetAllById()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_BY_ID_CACHE_KEY);
            Dictionary<string, Database> all = null;
            string query = @"
                SELECT d.oid as db_id, d.datname as db_name
                FROM pg_database d
            ";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<Database>(query, null).ToDictionary(x => $"{x.ID}");
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            return all;
        }
        private Dictionary<string, Database> GetAllByName()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_BY_ID_CACHE_KEY);
            Dictionary<string, Database> all = null;
            string query = @"
                SELECT d.oid as db_id, d.datname as db_name
                FROM pg_database d
            ";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<Database>(query, null).ToDictionary(x => $"{x.Name}");
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            return all;
        }
    }
}
