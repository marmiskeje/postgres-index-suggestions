using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface IAttributeHPartitioningDesigner
    {
        HPartitioningAttributeDefinition DesignFor(AttributeData attribute, ISet<string> operators, PrimaryKeyData relationPrimaryKey);
    }
}
