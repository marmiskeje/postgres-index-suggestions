using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    /// <summary>
    /// Currently contains attributes supporting comparison operators.
    /// When more granularity per operator is desirable, refactoring is needed.
    /// </summary>
    internal class StatementQueryExtractedData
    {
        public HashSet<IndexAttribute> WhereAttributes { get; set; }
        public HashSet<IndexAttribute> JoinAttributes { get; set; }
        public HashSet<IndexAttribute> GroupByAttributes { get; set; }
        public HashSet<IndexAttribute> OrderByAttributes { get; set; }
        public HashSet<IndexAttribute> ProjectionAttributes { get; set; }
    }
}
