using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IStoredProceduresRepository
    {
        IEnumerable<IStoredProcedure> GetAllNonSystemProcedures();
    }
}
