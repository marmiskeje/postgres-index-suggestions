using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface ISqlCreateStatementGenerator
    {
        VirtualIndexDefinition Generate(IndexDefinition indexDefinition, string filterExpression = null);
        VirtualHPartitioningDefinition Generate(HPartitioningDefinition hPartitioningDefinition);
    }
}
