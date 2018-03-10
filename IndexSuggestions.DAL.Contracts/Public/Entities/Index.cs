using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class Index
    {
        public long ID { get; set; }
        public List<NormalizedStatementIndexUsage> NormalizedStatementIndexUsages { get; set; }
    }
}
