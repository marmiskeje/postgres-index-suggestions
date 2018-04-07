using IndexSuggestions.DAL.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    public sealed class RepositoriesFactory : IRepositoriesFactory
    {
        private readonly DalSettings dalSettings = null;
        private static readonly Lazy<IRepositoriesFactory> instance = new Lazy<IRepositoriesFactory>(() => new RepositoriesFactory()); // default thread-safe

        public static IRepositoriesFactory Instance { get { return instance.Value; } }

        static RepositoriesFactory()
        {

        }

        private RepositoriesFactory()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("dalsettings.json")
                .Build();
            dalSettings = configuration.Get<DalSettings>();
        }

        private IndexSuggestionsContext CreateContext()
        {
            var context = new IndexSuggestionsContext(dalSettings.DBConnection.ProviderName, dalSettings.DBConnection.ConnectionString);
            return context;
        }

        public IIndicesRepository GetIndicesRepository()
        {
            return new IndicesRepository(CreateContext);
        }

        public INormalizedStatementIndexUsagesRepository GetNormalizedStatementIndexUsagesRepository()
        {
            return new NormalizedStatementIndexUsagesRepository(CreateContext);
        }

        public INormalizedStatementsRepository GetNormalizedStatementsRepository()
        {
            return new NormalizedStatementsRepository(CreateContext);
        }

        public ISettingPropertiesRepository GetSettingPropertiesRepository()
        {
            return new SettingPropertiesRepository(CreateContext);
        }

        public IWorkloadsRepository GetWorkloadsRepository()
        {
            return new WorkloadsRepository(CreateContext);
        }

        public INormalizedWorkloadStatementsRepository GetNormalizedWorkloadStatementsRepository()
        {
            return new NormalizedWorkloadStatementsRepository(CreateContext);
        }
    }
}
