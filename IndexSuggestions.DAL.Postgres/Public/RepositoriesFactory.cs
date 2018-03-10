using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    public sealed class RepositoriesFactory : IRepositoriesFactory
    {
        private static readonly Lazy<IRepositoriesFactory> instance = new Lazy<IRepositoriesFactory>(() => new RepositoriesFactory()); // default thread-safe
        private static Type dependency;

        public static IRepositoriesFactory Instance { get { return instance.Value; } }

        static RepositoriesFactory()
        {

        }

        private RepositoriesFactory()
        {

        }

        private IndexSuggestionsContext CreateContext()
        {
            var context = new IndexSuggestionsContext();
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
    }
}
