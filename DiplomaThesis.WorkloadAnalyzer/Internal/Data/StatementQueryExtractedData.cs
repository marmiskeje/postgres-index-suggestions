using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class StatementQueryExtractedData
    {
        private readonly SetOfAttributes whereAttributes;
        private readonly SetOfAttributes joinAttributes;
        private readonly SetOfAttributes groupByAttributes;
        private readonly SetOfAttributes orderByAttributes;
        private readonly SetOfAttributes projectionAttributes;
        public SetOfAttributes WhereAttributes
        {
            get { return whereAttributes; }
        }
        public SetOfAttributes JoinAttributes
        {
            get { return joinAttributes; }
        }
        public SetOfAttributes GroupByAttributes
        {
            get { return groupByAttributes; }
        }
        public SetOfAttributes OrderByAttributes
        {
            get { return orderByAttributes; }
        }
        public SetOfAttributes ProjectionAttributes
        {
            get { return projectionAttributes; }
        }
        public StatementQueryExtractedData(SetOfAttributes whereAttributes,
                                           SetOfAttributes joinAttributes,
                                           SetOfAttributes groupByAttributes,
                                           SetOfAttributes orderByAttributes,
                                           SetOfAttributes projectionAttributes)
        {
            this.whereAttributes = whereAttributes;
            this.joinAttributes = joinAttributes;
            this.groupByAttributes = groupByAttributes;
            this.orderByAttributes = orderByAttributes;
            this.projectionAttributes = projectionAttributes;
        }
    }
}
