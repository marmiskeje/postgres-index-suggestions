using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IPossibleIndicesRepository : IBaseRepository<long, PossibleIndex>
    {
        IEnumerable<PossibleIndex> GetByIds(IEnumerable<long> ids);
    }
}
