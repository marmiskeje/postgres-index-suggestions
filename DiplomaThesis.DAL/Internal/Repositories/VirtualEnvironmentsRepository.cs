using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentsRepository : BaseRepository<long, VirtualEnvironment>, IVirtualEnvironmentsRepository
    {
        public VirtualEnvironmentsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }
    }
}
