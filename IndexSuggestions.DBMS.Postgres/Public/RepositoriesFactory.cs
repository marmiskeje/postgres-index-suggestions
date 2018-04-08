using IndexSuggestions.DBMS.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    public sealed class RepositoriesFactory : IRepositoriesFactory
    {
        private readonly DBMSSettings settings = null;
        private static readonly Lazy<IRepositoriesFactory> instance = new Lazy<IRepositoriesFactory>(() => new RepositoriesFactory()); // default thread-safe

        public static IRepositoriesFactory Instance { get { return instance.Value; } }

        static RepositoriesFactory()
        {

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
    }
}
