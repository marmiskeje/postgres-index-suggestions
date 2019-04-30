using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class DBMSSettings
    {
        public DBConnection DBConnection { get; set; }
    }

    internal class DBConnection
    {
        public string ConnectionString { get; set; }
    }
}
