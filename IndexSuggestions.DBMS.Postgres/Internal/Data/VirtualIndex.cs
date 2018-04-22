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
}
