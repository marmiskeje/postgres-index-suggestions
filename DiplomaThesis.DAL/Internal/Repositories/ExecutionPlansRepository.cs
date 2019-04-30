using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class ExecutionPlansRepository : BaseRepository<long, ExecutionPlan>, IExecutionPlansRepository
    {
        public ExecutionPlansRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            CacheExpiration = TimeSpan.FromMinutes(5);
        }

    }
}
