using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelationAttribute
    {
        uint ID { get; set; }
        int AttributeNumber { get; set; }
        uint RelationID { get; set; }
        string Name { get; set; }
    }
}
