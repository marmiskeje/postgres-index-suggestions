using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class RawSqlExecutionRepository : BaseRepository, IRawSqlExecutionRepository
    {
        public RawSqlExecutionRepository(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<T> ExecuteQuery<T>(string sql)
        {
            return base.ExecuteQuery<T>(sql, null);
        }
    }
}
