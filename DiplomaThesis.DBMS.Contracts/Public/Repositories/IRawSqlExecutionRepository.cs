using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IRawSqlExecutionRepository
    {
        IEnumerable<T> ExecuteQuery<T>(string sql);
    }
}
