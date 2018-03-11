﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedStatement : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [MaxLength(4000)]
        public string Statement { get; set; }
        public List<NormalizedStatementIndexUsage> NormalizedStatementIndexUsages { get; set; }
    }
}
