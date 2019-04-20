using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class PossibleIndicesRepository : BaseRepository<long, PossibleIndex>, IPossibleIndicesRepository
    {
        public PossibleIndicesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
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
    }
}
