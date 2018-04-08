using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public class RelationAttribute
    {
        public long ID { get; set; }
        public long Position { get; set; }
        public long RelationID { get; set; }
        public string Name { get; set; }
    }
}
