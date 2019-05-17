using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class PossibleIndicesRepository : BaseRepository<long, PossibleIndex>, IPossibleIndicesRepository
    {
        public PossibleIndicesRepository(Func<DiplomaThesisContext> createContextFunc) : base(createContextFunc)
        {

        }

        protected override void FillEntitySet(PossibleIndex entity)
        {
            base.FillEntitySet(entity);
            entity.FilterExpressionsData = JsonSerializationUtility.Serialize(entity.FilterExpressions);
        }

        protected override void FillEntityGet(PossibleIndex entity)
        {
            base.FillEntityGet(entity);
            entity.FilterExpressions = JsonSerializationUtility.Deserialize<PossibleIndexFilterExpressionsData>(entity.FilterExpressionsData);
        }

        public IEnumerable<PossibleIndex> GetByIds(IEnumerable<long> ids)
        {
            using (var context = CreateContextFunc())
            {
                var result = context.PossibleIndices.Where(x => ids.Contains(x.ID)).ToList();
                result.ForEach(x => FillEntityGet(x));
                return result;
            }
        }
    }
}
