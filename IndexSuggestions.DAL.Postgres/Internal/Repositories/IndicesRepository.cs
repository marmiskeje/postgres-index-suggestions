using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    internal class IndicesRepository : BaseRepository<long, Index>, IIndicesRepository
    {
        public IndicesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
