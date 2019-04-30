using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IIndex
    {
        uint ID { get; }
        string Name { get; }
        IList<String> AttributesNames { get; }
        uint RelationID { get; }
        string RelationName { get; }
        uint SchemaID { get; }
        string SchemaName { get; }
        uint DatabaseID { get; }
        string DatabaseName { get; }
        IndexAccessMethodType AccessMethod { get;}
        string CreateDefinition { get; }
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
