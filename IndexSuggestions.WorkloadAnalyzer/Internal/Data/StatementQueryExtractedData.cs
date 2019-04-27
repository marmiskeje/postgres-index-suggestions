using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.WorkloadAnalyzer
{
    /// <summary>
    /// Currently contains attributes supporting comparison operators.
    /// When more granularity per operator is desirable, refactoring is needed.
    /// </summary>
    internal class StatementQueryExtractedData
    {
        private readonly ISet<AttributeData> whereAttributes;
        private readonly ISet<AttributeData> joinAttributes;
        private readonly ISet<AttributeData> groupByAttributes;
        private readonly ISet<AttributeData> orderByAttributes;
        private readonly Dictionary<AttributeData, ISet<string>> whereOperatorsByAttribute;
        private readonly Dictionary<AttributeData, ISet<string>> joinOperatorsByAttribute;
        private readonly Dictionary<AttributeData, ISet<string>> groupByOperatorsByAttribute;
        private readonly Dictionary<AttributeData, ISet<string>> orderByOperatorsByAttribute;
        private readonly ISet<AttributeData> projectionAttributes;
        public ISet<AttributeData> WhereAttributes
        {
            get { return whereAttributes; }
        }
        public IReadOnlyDictionary<AttributeData, ISet<string>> WhereOperatorsByAttribute
        {
            get { return whereOperatorsByAttribute; }
        }
        public ISet<AttributeData> JoinAttributes
        {
            get { return joinAttributes; }
        }
        public IReadOnlyDictionary<AttributeData, ISet<string>> JoinOperatorsByAttribute
        {
            get { return whereOperatorsByAttribute; }
        }
        public ISet<AttributeData> GroupByAttributes
        {
            get { return groupByAttributes; }
        }
        public IReadOnlyDictionary<AttributeData, ISet<string>> GroupByOperatorsByAttribute
        {
            get { return whereOperatorsByAttribute; }
        }
        public ISet<AttributeData> OrderByAttributes
        {
            get { return orderByAttributes; }
        }
        public IReadOnlyDictionary<AttributeData, ISet<string>> OrderByOperatorsByAttribute
        {
            get { return whereOperatorsByAttribute; }
        }
        public ISet<AttributeData> ProjectionAttributes
        {
            get { return projectionAttributes; }
        }
        public StatementQueryExtractedData(Dictionary<AttributeData, ISet<string>> whereOperatorsByAttribute,
                                           Dictionary<AttributeData, ISet<string>> joinOperatorsByAttribute,
                                           Dictionary<AttributeData, ISet<string>> groupByOperatorsByAttribute,
                                           Dictionary<AttributeData, ISet<string>> orderByOperatorsByAttribute,
                                           ISet<AttributeData> projectionAttributes)
        {
            this.whereOperatorsByAttribute = whereOperatorsByAttribute;
            this.whereAttributes = whereOperatorsByAttribute.Keys.ToHashSet();
            this.joinOperatorsByAttribute = joinOperatorsByAttribute;
            this.joinAttributes = joinOperatorsByAttribute.Keys.ToHashSet();
            this.groupByOperatorsByAttribute = groupByOperatorsByAttribute;
            this.groupByAttributes = groupByOperatorsByAttribute.Keys.ToHashSet();
            this.orderByOperatorsByAttribute = orderByOperatorsByAttribute;
            this.orderByAttributes = orderByOperatorsByAttribute.Keys.ToHashSet();
            this.projectionAttributes = projectionAttributes;
        }
    }
}
