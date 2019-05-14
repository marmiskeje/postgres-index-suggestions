using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public class VirtualHPartitioningDefinition
    {
        public string SchemaName { get; set; }
        public string RelationName { get; set; }
        public string PartitioningStatement { get; set; }
        public HashSet<string> PartitionStatements { get; set; }
    }
}
