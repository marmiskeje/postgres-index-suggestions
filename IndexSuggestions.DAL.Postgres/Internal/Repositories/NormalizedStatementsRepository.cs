using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    internal class NormalizedStatementsRepository : BaseRepository<long, NormalizedStatement>, INormalizedStatementsRepository
    {
        public NormalizedStatementsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        public NormalizedStatement GetByStatement(string statement)
        {
            using (var context = CreateContextFunc())
            {
                return context.NormalizedStatements.Where(x => x.Statement == statement).SingleOrDefault();
            }
        }
    }
}
