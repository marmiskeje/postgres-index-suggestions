using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IWorkloadsRepository : IBaseRepository<long, Workload>
    {
        IEnumerable<Workload> GetForDatabase(uint databaseID, DateTime createdDateFromInclusive, DateTime createdDateToExlusive, bool onlyActive = true);
    }
}
