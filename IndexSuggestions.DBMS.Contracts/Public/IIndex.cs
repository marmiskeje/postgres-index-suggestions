using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IIndex
    {
        uint ID { get; set; }
        string Name { get; set; }
        IList<String> AttributesNames { get; set; }
        uint RelationID { get; set; }
        string RelationName { get; set; }
        uint SchemaID { get; set; }
        string SchemaName { get; set; }
        uint DatabaseID { get; set; }
        string DatabaseName { get; set; }
        IndexAccessMethodType AccessMethod { get; set; }
    }

    public enum IndexAccessMethodType
    {
        Unknown = 0,
        BTree = 1,
        Hash = 2,
        Gist = 3,
        Gin = 4,
        SpGist = 5,
        Brin = 6
    }
}
