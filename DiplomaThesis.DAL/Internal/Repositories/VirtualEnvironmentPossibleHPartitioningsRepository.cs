using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentPossibleHPartitioningsRepository : BaseRepository<long, VirtualEnvironmentPossibleHPartitioning>, IVirtualEnvironmentPossibleHPartitioningsRepository
    {
        public VirtualEnvironmentPossibleHPartitioningsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
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
