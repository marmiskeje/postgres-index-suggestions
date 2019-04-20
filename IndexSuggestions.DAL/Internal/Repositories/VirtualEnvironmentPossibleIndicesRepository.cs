using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.DAL
{
    internal class VirtualEnvironmentPossibleIndicesRepository : BaseSimpleRepository<VirtualEnvironmentPossibleIndex>, IVirtualEnvironmentPossibleIndicesRepository
    {
        public VirtualEnvironmentPossibleIndicesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
