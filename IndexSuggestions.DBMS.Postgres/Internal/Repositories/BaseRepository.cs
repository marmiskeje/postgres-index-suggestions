using Npgsql;
using System;
using System.Collections.Generic;
using Dapper;
using IndexSuggestions.Common.Cache;

namespace IndexSuggestions.DBMS.Postgres
{
    internal abstract class BaseRepository
    {
        protected Type ThisType { get; private set; }
        protected ICache Cache
        {
            get { return CacheProvider.Instance.MemoryCache; }
        }
        protected string ConnectionString { get; private set; }
        protected int DefaultCommandTimeout { get; private set; }
        public BaseRepository(string connectionString) : this(connectionString, 60)
        {

        }
        public BaseRepository(string connectionString, int defaultCommandTimeout)
        {
            ThisType = GetType();
            ConnectionString = connectionString;
            DefaultCommandTimeout = defaultCommandTimeout;
        }

        protected virtual NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        protected IEnumerable<T> ExecuteQuery<T>(string queryText, object parametersContainer, int? commandTimeout = null)
        {
            int commandTimeoutToUse = commandTimeout ?? DefaultCommandTimeout;
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(queryText, parametersContainer, commandTimeout: commandTimeoutToUse, commandType: System.Data.CommandType.Text);
            }
        }

        protected int Execute(string queryText, object parametersContainer, int? commandTimeout = null)
        {
            int commandTimeoutToUse = commandTimeout ?? DefaultCommandTimeout;
            using (var connection = CreateConnection())
            {
                return connection.Execute(queryText, parametersContainer, commandTimeout: commandTimeoutToUse, commandType: System.Data.CommandType.Text);
            }
        }

        protected string CreateCacheKeyForThisType(string uniqueKey)
        {
            return String.Format("{0}:{1}_{2}", ThisType.Assembly.FullName, ThisType.FullName, uniqueKey);
        }
    }
}
