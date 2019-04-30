using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IDatabaseDependencyObjectsRepository
    {
        IEnumerable<IDatabaseDependencyObject> GetDependenciesForDatabaseObject(uint id);
    }
}
