using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IWorkloadAnalysesRepository : IBaseRepository<long, WorkloadAnalysis>
    {

        IEnumerable<WorkloadAnalysis> LoadForProcessing(int maxCount);
    }
}
