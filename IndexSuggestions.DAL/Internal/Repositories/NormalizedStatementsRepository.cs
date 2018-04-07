﻿using IndexSuggestions.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal class NormalizedStatementsRepository : BaseRepository<long, NormalizedStatement>, INormalizedStatementsRepository
    {
        public NormalizedStatementsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public NormalizedStatement GetByStatementFingerprint(string fingerprint, bool useCache = false)
        {
            return Get(() =>
            {
                using (var context = CreateContextFunc())
                {
                    return context.NormalizedStatements.Where(x => x.StatementFingerprint == fingerprint).SingleOrDefault();
                }
            }, fingerprint, useCache);
        }

        protected override void FillEntityGet(NormalizedStatement entity)
        {
            base.FillEntityGet(entity);
            if (entity.StatementDefinitionData != null)
            {
                entity.StatementDefinition = JsonSerializationUtility.Deserialize<StatementDefinition>(entity.StatementDefinitionData); 
            }
        }

        protected override void FillEntitySet(NormalizedStatement entity)
        {
            base.FillEntitySet(entity);
            if (entity.StatementDefinition != null)
            {
                entity.StatementDefinitionData = JsonSerializationUtility.Serialize(entity.StatementDefinition);
            }
        }

        protected override ISet<string> GetAllCacheKeys(long key, NormalizedStatement entity)
        {
            var result = new HashSet<string>(new[] { entity.StatementFingerprint });
            result.UnionWith(base.GetAllCacheKeys(key, entity));
            return result;
        }

    }
}
