using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IDbObjectDefinitionGenerator
    {
        VirtualIndexDefinition Generate(IndexDefinition indexDefinition, string filterExpression = null);
        VirtualHPartitioningDefinition Generate(HPartitioningDefinition hPartitioningDefinition);
    }
}
