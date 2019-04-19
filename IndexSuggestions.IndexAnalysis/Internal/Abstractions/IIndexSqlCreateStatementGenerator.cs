using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal interface IIndexSqlCreateStatementGenerator
    {
        string Generate(IndexDefinition indexDefinition);
    }
}
