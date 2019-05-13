using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class ConfigurationData
    {
        public ConfigurationCollectorData Collector { get; set; }
        public ConfigurationReportsData Reports { get; set; }
        public ConfigurationSmtpData Smtp { get; set; }
    }

    public class ConfigurationCollectorData
    {
        public List<ConfigurationCollectorDatabaseData> Databases { get; set; } = new List<ConfigurationCollectorDatabaseData>();
    }

    public class ConfigurationCollectorDatabaseData
    {
        public uint DatabaseID { get; set; }
        public bool IsEnabledGeneralCollection { get; set; }
        public bool IsEnabledStatementCollection { get; set; }
    }

    public class ConfigurationReportsData
    {
        public string EmailAddresses { get; set; }
    }

    public class ConfigurationSmtpData
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
