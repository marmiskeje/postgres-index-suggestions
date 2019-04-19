using Npgsql;
using System;
using System.Collections.Generic;
using Dapper;
using IndexSuggestions.Common.Cache;
using static Dapper.SqlMapper;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.DBMS.Postgres
{
    internal interface IDataReader
    {
        IEnumerable<T> Read<T>();
        T ReadSingle<T>();
    }

    internal class DataReader : IDataReader
    {
        private readonly GridReader reader;
        public DataReader(GridReader reader)
        {
            this.reader = reader;
        }
        public IEnumerable<T> Read<T>()
        {
            return reader.Read<T>();
        }
        public T ReadSingle<T>()
        {
            return reader.ReadSingle<T>();
        }
    }

    internal abstract class BaseRepository
    {
        protected Type ThisType { get; private set; }
        protected ICache Cache
        {
            get { return CacheProvider.Instance.MemoryCache; }
        }
        protected string ConnectionString { get; private set; }
        protected int DefaultCommandTimeout { get; private set; }
        protected TimeSpan ShortCacheExpiration { get; } = TimeSpan.FromMinutes(5);
        protected TimeSpan MediumCacheExpiration { get; } = TimeSpan.FromMinutes(10);
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
            var connectionStringToUse = ConnectionString;
            if (!String.IsNullOrEmpty(DatabaseScope.Current))
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionStringToUse);
                connectionStringBuilder.Database = DatabaseScope.Current;
                connectionStringToUse = connectionStringBuilder.ConnectionString;
            }
            return new NpgsqlConnection(connectionStringToUse);
        }

        protected void ExecuteQueryMulti(string queryText, object parametersContainer, Action<IDataReader> readerAction, int? commandTimeout = null)
        {
            int commandTimeoutToUse = commandTimeout ?? DefaultCommandTimeout;
            using (var connection = CreateConnection())
            {
                using (var reader = connection.QueryMultiple(queryText, parametersContainer, commandTimeout: commandTimeoutToUse, commandType: System.Data.CommandType.Text))
                {
                    readerAction(new DataReader(reader));
                }
            }
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
            var dbName = DatabaseScope.Current ?? "Implicit";
            return String.Format("{0}_{1}:{2}_{3}", dbName, ThisType.Assembly.FullName, ThisType.FullName, uniqueKey);
        }
    }
}
