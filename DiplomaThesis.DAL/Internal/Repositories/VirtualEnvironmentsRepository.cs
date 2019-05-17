using DiplomaThesis.DAL.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentsRepository : BaseRepository<long, VirtualEnvironment>, IVirtualEnvironmentsRepository
    {
        public VirtualEnvironmentsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {

        }

        public IEnumerable<VirtualEnvironment> GetAllForWorkloadAnalysis(long workloadAnalysisID, VirtualEnvironmentType type)
        {
            using (var context = CreateContextFunc())
            {
                var query = context.VirtualEnvironments.Where(x => x.WorkloadAnalysisID == workloadAnalysisID && x.Type == type);
                if (type == VirtualEnvironmentType.Indices)
                {
                    query = query.Include(x => x.VirtualEnvironmentPossibleIndices);
                }
                else if (type == VirtualEnvironmentType.HPartitionings)
                {
                    query = query.Include(x => x.VirtualEnvironmentPossibleHPartitionings);
                }
                var result = query.ToList();
                if (type == VirtualEnvironmentType.HPartitionings)
                {
                    result.SelectMany(x => x.VirtualEnvironmentPossibleHPartitionings).ToList().ForEach(x => x.PartitionStatements = JsonSerializationUtility.Deserialize<HashSet<string>>(x.PartitionStatementsData));
                }
                return result;
            }
        }

        public long GetBestEnvironmentForWorkloadAnalysis(long workloadAnalysisID)
        {
            using (var context = CreateContextFunc())
            {
                return (from e in context.VirtualEnvironments
                        join i in context.VirtualEnvironmentPossibleIndices on e.ID equals i.VirtualEnvironemntID
                        where e.WorkloadAnalysisID == workloadAnalysisID
                        group i by i.VirtualEnvironemntID into g
                        orderby g.Sum(x => x.ImprovementRatio) descending
                        select new { Key = g.Key }).Select(x => x.Key).FirstOrDefault();
            }
        }

        public VirtualEnvironment GetDetail(long environmentID)
        {
            using (var context = CreateContextFunc())
            {
                var result = context.VirtualEnvironments.Include(x => x.VirtualEnvironmentPossibleCoveringIndices)
                    .Include(x => x.VirtualEnvironmentStatementEvaluations).ThenInclude(x => x.ExecutionPlan)
                    .Where(x => x.ID == environmentID).SingleOrDefault();
                result.VirtualEnvironmentStatementEvaluations.ForEach(x =>
                {
                    x.AffectingIndices = JsonSerializationUtility.Deserialize<HashSet<long>>(x.AffectingIndicesData);
                    x.UsedIndices = JsonSerializationUtility.Deserialize<HashSet<long>>(x.UsedIndicesData);
                });
                return result;
            }
        }
    }
}
