using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class PrimaryKeyData
    {
        public IReadOnlyList<AttributeData> Attributes { get; }
        public PrimaryKeyData(IEnumerable<AttributeData> attributes)
        {
            Attributes = new List<AttributeData>(attributes);
        }
    }
}
