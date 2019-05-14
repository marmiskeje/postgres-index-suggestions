using DiplomaThesis.DBMS.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    public sealed class RepositoriesFactory : IRepositoriesFactory
    {
        private readonly DBMSSettings settings = null;
        private static readonly Lazy<IRepositoriesFactory> instance = new Lazy<IRepositoriesFactory>(() => new RepositoriesFactory()); // default thread-safe

        public static IRepositoriesFactory Instance { get { return instance.Value; } }

        static RepositoriesFactory()
        {
            Dapper.SqlMapper.SetTypeMap(typeof(Relation), new DapperColumnAttributeTypeMapper<Relation>());
            Dapper.SqlMapper.SetTypeMap(typeof(RelationAttribute), new DapperColumnAttributeTypeMapper<RelationAttribute>());
            Dapper.SqlMapper.SetTypeMap(typeof(Index), new DapperColumnAttributeTypeMapper<Index>());
            Dapper.SqlMapper.SetTypeMap(typeof(ExplainResultDbData), new DapperColumnAttributeTypeMapper<ExplainResultDbData>());
            Dapper.SqlMapper.SetTypeMap(typeof(VirtualIndex), new DapperColumnAttributeTypeMapper<VirtualIndex>());
            Dapper.SqlMapper.SetTypeMap(typeof(Database), new DapperColumnAttributeTypeMapper<Database>());
            Dapper.SqlMapper.SetTypeMap(typeof(DatabaseStatistics), new DapperColumnAttributeTypeMapper<DatabaseStatistics>());
            Dapper.SqlMapper.SetTypeMap(typeof(IndexStatistics), new DapperColumnAttributeTypeMapper<IndexStatistics>());
            Dapper.SqlMapper.SetTypeMap(typeof(RelationStatistics), new DapperColumnAttributeTypeMapper<RelationStatistics>());
            Dapper.SqlMapper.SetTypeMap(typeof(StoredProcedureStatistics), new DapperColumnAttributeTypeMapper<StoredProcedureStatistics>());
            Dapper.SqlMapper.SetTypeMap(typeof(TotalDatabaseCollectionInfo), new DapperColumnAttributeTypeMapper<TotalDatabaseCollectionInfo>());
            Dapper.SqlMapper.SetTypeMap(typeof(View), new DapperColumnAttributeTypeMapper<View>());
            Dapper.SqlMapper.SetTypeMap(typeof(StoredProcedure), new DapperColumnAttributeTypeMapper<StoredProcedure>());
            Dapper.SqlMapper.SetTypeMap(typeof(ExpressionOperator), new DapperColumnAttributeTypeMapper<ExpressionOperator>());
            Dapper.SqlMapper.SetTypeMap(typeof(DatabaseSystemInfo), new DapperColumnAttributeTypeMapper<DatabaseSystemInfo>());
            Dapper.SqlMapper.SetTypeMap(typeof(VirtualIndexSize), new DapperColumnAttributeTypeMapper<VirtualIndexSize>());
        }

        private RepositoriesFactory()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("dbmssettings.json")
                .Build();
            settings = configuration.Get<DBMSSettings>();
            var connectionStringSplit = settings.DBConnection.ConnectionString.Split(";");
            bool containsAppName = false;
            foreach (var keyValue in connectionStringSplit)
            {
                var keyValueSplit = keyValue.Split("=");
                if (keyValueSplit[0].Trim().Replace(" ", "").ToLower() == "applicationname" && keyValueSplit[1].Trim() == "IndexSuggestions")
                {
                    containsAppName = true;
                    break;
                }
            }
            if (!containsAppName)
            {
                throw new ArgumentException("ConnectionString must contain ApplicationName=IndexSuggestions!"); // otherwise infinite log processing may occur
            }
        }

        public IRelationAttributesRepository GetRelationAttributesRepository()
        {
            return new RelationAttributesRepository(settings.DBConnection.ConnectionString);
        }

        public IRelationsRepository GetRelationsRepository()
        {
            return new RelationsRepository(settings.DBConnection.ConnectionString);
        }

        public IIndicesRepository GetIndicesRepository()
        {
            return new IndicesRepository(settings.DBConnection.ConnectionString);
        }

        public IExplainRepository GetExplainRepository()
        {
            return new ExplainRepository(settings.DBConnection.ConnectionString);
        }

        public IVirtualIndicesRepository GetVirtualIndicesRepository()
        {
            return new VirtualIndicesRepository(settings.DBConnection.ConnectionString);
        }

        public IDatabasesRepository GetDatabasesRepository()
        {
            return new DatabasesRepository(settings.DBConnection.ConnectionString);
        }

        public ITotalDatabaseStatisticsRepository GetTotalDatabaseStatisticsRepository()
        {
            return new TotalDatabaseStatisticsRepository(settings.DBConnection.ConnectionString);
        }

        public IViewsRepository GetViewsRepository()
        {
            return new ViewsRepository(settings.DBConnection.ConnectionString);
        }

        public IDatabaseDependencyObjectsRepository GetDatabaseDependencyObjectsRepository()
        {
            return new DatabaseDependencyObjectsRepository(settings.DBConnection.ConnectionString);
        }

        public IStoredProceduresRepository GetStoredProceduresRepository()
        {
            return new StoredProceduresRepository(settings.DBConnection.ConnectionString);
        }

        public IExpressionOperatorsRepository GetExpressionOperatorsRepository()
        {
            return new ExpressionOperatorsRepository(settings.DBConnection.ConnectionString);
        }

        public IDatabaseSystemInfoRepository GetDatabaseSystemInfoRepository()
        {
            return new DatabaseSystemInfoRepository(settings.DBConnection.ConnectionString);
        }

        public IVirtualHPartitioningsRepository GetVirtualHPartitioningsRepository()
        {
            return new VirtualHPartitioningsRepository(settings.DBConnection.ConnectionString);
        }

        public IRawSqlExecutionRepository GetRawSqlExecutionRepository()
        {
            return new RawSqlExecutionRepository(settings.DBConnection.ConnectionString);
        }
    }
}
