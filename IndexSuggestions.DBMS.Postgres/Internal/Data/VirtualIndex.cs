using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class VirtualIndex : IVirtualIndex
    {
        [Column("indexrelid")]
        public uint ID { get; set; }
        [Column("indexname")]
        public string Name { get; set; }
    }

    internal class VirtualIndexSize
    {
        [Column("hypopg_relation_size")]
        public long Bytes { get; set; }
    }
}
