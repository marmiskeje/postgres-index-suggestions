using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public class DatabaseScope : IDisposable
    {
        [ThreadStatic]
        private static string databaseName = null;
        public static string Current
        {
            get { return databaseName; }
        }
        public DatabaseScope(string databaseName)
        {
            DatabaseScope.databaseName = databaseName;
        }
        public void Dispose()
        {
            databaseName = null;
        }
    }
}
