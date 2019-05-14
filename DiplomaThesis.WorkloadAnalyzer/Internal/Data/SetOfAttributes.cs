using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class SetOfAttributes
    {
        public ISet<AttributeData> All { get; }
        public ISet<AttributeData> BTreeApplicable { get; }
        public IReadOnlyDictionary<AttributeData, ISet<string>> AllOperatorsByAttribute { get; }

        public SetOfAttributes(IReadOnlyDictionary<AttributeData, ISet<string>> allOperatorsByAttribute, ISet<AttributeData> bTreeApplicable)
        {
            All = new HashSet<AttributeData>(allOperatorsByAttribute.Keys);
            BTreeApplicable = bTreeApplicable;
            AllOperatorsByAttribute = allOperatorsByAttribute;
        }
    }
}
