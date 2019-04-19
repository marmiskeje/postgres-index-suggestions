using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    public class PostgresDbTypeIdentificationAttribute : Attribute
    {
        public string Name { get; private set; }
        public uint OID { get; private set; }

        public PostgresDbTypeIdentificationAttribute(string name, uint oid)
        {
            Name = name;
            OID = oid;
        }
    }
}
