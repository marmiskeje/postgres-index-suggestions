using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IWorkloadAnalysesRepository : IBaseRepository<long, WorkloadAnalysis>
    {

        IEnumerable<WorkloadAnalysis> LoadForProcessing(int maxCount);
        IEnumerable<WorkloadAnalysis> GetForDatabase(uint databaseID, DateTime createdDateFromInclusive, DateTime createdDateToExlusive);
        WorkloadAnalysis Get(long workloadAnalysisID);
    }
}
