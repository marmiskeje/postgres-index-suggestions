using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class ExplainResult : IExplainResult
    {
        [Column("QUERY PLAN")]
        public string PlanJson { get; set; }
    }
}
