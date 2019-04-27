using IndexSuggestions.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.DAL
{
    internal class VirtualEnvironmentPossibleHPartitioningsRepository : BaseRepository<long, VirtualEnvironmentPossibleHPartitioning>, IVirtualEnvironmentPossibleHPartitioningsRepository
    {
        public VirtualEnvironmentPossibleHPartitioningsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        protected override void FillEntitySet(VirtualEnvironmentPossibleHPartitioning entity)
        {
            base.FillEntitySet(entity);
            entity.PartitionStatementsData = JsonSerializationUtility.Serialize(entity.PartitionStatements);
        }

        protected override void FillEntityGet(VirtualEnvironmentPossibleHPartitioning entity)
        {
            base.FillEntityGet(entity);
            entity.PartitionStatements = JsonSerializationUtility.Deserialize<HashSet<string>>(entity.PartitionStatementsData);
        }
    }
}
