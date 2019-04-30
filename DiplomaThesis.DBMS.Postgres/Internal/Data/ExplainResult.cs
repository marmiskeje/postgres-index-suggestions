using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class ExplainResult : IExplainResult
    {
        public string PlanJson { get; set; }

        public QueryPlanNode Plan { get; set; }

        public ISet<string> UsedIndexScanIndices { get; } = new HashSet<string>();
    }
    internal class ExplainResultDbData
    {
        [Column("QUERY PLAN")]
        public string PlanJson { get; set; }
    }
}
