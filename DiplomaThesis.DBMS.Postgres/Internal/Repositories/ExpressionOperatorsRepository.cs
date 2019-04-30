using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class ExpressionOperatorsRepository : BaseRepository, IExpressionOperatorsRepository
    {
        private const string ALL_BY_ID_CACHE_KEY = "ALL_BY_ID";
        public ExpressionOperatorsRepository(string connectionString) : base(connectionString)
        {

        }
        public IExpressionOperator Get(uint operatorID)
        {
            string attributeKey = $"{operatorID}";
            IExpressionOperator result = null;
            Dictionary<string, ExpressionOperator> all = GetAllById();
            if (all.ContainsKey(attributeKey))
            {
                result = all[attributeKey];
            }
            return result;
        }

        private Dictionary<string, ExpressionOperator> GetAllById()
        {
            var cacheKey = CreateCacheKeyForThisType(ALL_BY_ID_CACHE_KEY);
            Dictionary<string, ExpressionOperator> all = null;
            string query = @"
                select oid op_id, oprname op_name from pg_catalog.pg_operator order by oid
            ";
            if (!Cache.TryGetValue(cacheKey, out all))
            {
                all = ExecuteQuery<ExpressionOperator>(query, null).ToDictionary(x => $"{x.ID}");
                Cache.Save(cacheKey, all, MediumCacheExpiration);
            }
            return all;
        }
    }
}
