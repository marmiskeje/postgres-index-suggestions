using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class DalSettings
    {
        public DBConnection DBConnection { get; set; }
    }

    internal class DBConnection
    {
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}
