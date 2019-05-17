using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal static class IndexApplicability
    {
        internal static bool IsIndexApplicableForQuery(StatementQueryExtractedData extractedData, IndexDefinition index)
        {
            return extractedData.WhereAttributes.All
                .Union(extractedData.JoinAttributes.All)
                .Union(extractedData.GroupByAttributes.All)
                .Union(extractedData.OrderByAttributes.All).Contains(index.Attributes.First());
        }
    }
}
