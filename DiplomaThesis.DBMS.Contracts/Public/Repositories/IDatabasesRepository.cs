using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IDatabasesRepository
    {
        IDatabase Get(uint databaseID);
        IDatabase GetByName(string name);
        IEnumerable<IDatabase> GetAll();
    }
}
