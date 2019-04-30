using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal interface IAttributeHPartitioningDesigner
    {
        HPartitioningAttributeDefinition DesignFor(AttributeData attribute, ISet<string> operators, PrimaryKeyData relationPrimaryKey);
    }
}
