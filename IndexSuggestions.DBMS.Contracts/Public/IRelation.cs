using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelation
    {
        uint ID { get; set; }
        string Name { get; set; }
        uint SchemaID { get; set; }
        string SchemaName { get; set; }
        uint DatabaseID { get; set; }
        string DatabaseName { get; set; }
    }
}
