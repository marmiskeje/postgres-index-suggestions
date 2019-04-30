using DiplomaThesis.DBMS.Contracts;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class DatabaseSystemInfoRepository : BaseRepository, IDatabaseSystemInfoRepository
    {
        public DatabaseSystemInfoRepository(string connectionString) : base(connectionString)
        {

        }
        public IDatabaseSystemInfo LoadInfo()
        {
            var query = "SELECT current_setting('server_version_num') version_number_string";
            return ExecuteQuery<DatabaseSystemInfo>(query, null).Single();
        }
    }
}
