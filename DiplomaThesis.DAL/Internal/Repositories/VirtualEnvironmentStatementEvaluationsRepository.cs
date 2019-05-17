using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class VirtualEnvironmentStatementEvaluationsRepository : BaseSimpleRepository<VirtualEnvironmentStatementEvaluation>, IVirtualEnvironmentStatementEvaluationsRepository
    {
        public VirtualEnvironmentStatementEvaluationsRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {

        }

        protected override void FillEntitySet(VirtualEnvironmentStatementEvaluation entity)
        {
            base.FillEntitySet(entity);
            entity.AffectingIndicesData = JsonSerializationUtility.Serialize(entity.AffectingIndices);
            entity.UsedIndicesData = JsonSerializationUtility.Serialize(entity.UsedIndices);
        }

        protected override void FillEntityGet(VirtualEnvironmentStatementEvaluation entity)
        {
            base.FillEntityGet(entity);
            entity.AffectingIndices = JsonSerializationUtility.Deserialize<HashSet<long>>(entity.AffectingIndicesData);
            entity.UsedIndices = JsonSerializationUtility.Deserialize<HashSet<long>>(entity.UsedIndicesData);
        }
    }
}
