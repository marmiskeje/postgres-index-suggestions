using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IVirtualEnvironmentsRepository : IBaseRepository<long, VirtualEnvironment>
    {
        IEnumerable<VirtualEnvironment> GetAllForWorkloadAnalysis(long workloadAnalysisID, VirtualEnvironmentType type);
        VirtualEnvironment GetDetail(long environmentID);
        long GetBestEnvironmentForWorkloadAnalysis(long workloadAnalysisID);
    }
}
