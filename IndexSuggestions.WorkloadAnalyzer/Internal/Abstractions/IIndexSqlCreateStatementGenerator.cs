using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface IIndexSqlCreateStatementGenerator
    {
        string Generate(IndexDefinition indexDefinition, string filterExpression = null);
    }
}
