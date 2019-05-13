using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class CollectorConfiguration
    {
        public Dictionary<uint, CollectorDatabaseConfiguration> Databases { get; set; } = new Dictionary<uint, CollectorDatabaseConfiguration>();
    }

    public class CollectorDatabaseConfiguration
    {
        public uint DatabaseID { get; set; }
        public bool IsEnabledGeneralCollection { get; set; }
        public bool IsEnabledStatementCollection { get; set; }
    }
}
