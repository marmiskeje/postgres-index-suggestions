using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentPossibleIndicesRepository : BaseSimpleRepository<VirtualEnvironmentPossibleIndex>, IVirtualEnvironmentPossibleIndicesRepository
    {
        public VirtualEnvironmentPossibleIndicesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
