using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal static class SystemObjects
    {
        private static IEnumerable<string> systemSchemas = new List<string>(new []{"information_schema", "pg_catalog"});
        public static IEnumerable<string> SystemSchemas
        {
            get { return systemSchemas; }
        }
    }
}
