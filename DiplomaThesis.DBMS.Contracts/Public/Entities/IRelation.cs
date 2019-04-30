using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IRelation
    {
        uint ID { get; }
        string Name { get;  }
        uint SchemaID { get; }
        string SchemaName { get; }
        uint DatabaseID { get;  }
        string DatabaseName { get; }
        long Size { get; }
        long TuplesCount { get; }
        IList<String> PrimaryKeyAttributeNames { get; }
    }
}
