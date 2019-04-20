using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.DAL
{
    internal class VirtualEnvironmentPossibleCoveringIndicesRepository : BaseSimpleRepository<VirtualEnvironmentPossibleCoveringIndex>, IVirtualEnvironmentPossibleCoveringIndicesRepository
    {
        public VirtualEnvironmentPossibleCoveringIndicesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
