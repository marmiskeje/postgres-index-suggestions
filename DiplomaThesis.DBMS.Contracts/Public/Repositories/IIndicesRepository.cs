using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IIndicesRepository
    {
        IEnumerable<IIndex> GetAll();
        IEnumerable<IIndex> GetAllNonSystems();
    }
}
