using IndexSuggestions.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal class QueryTreeProvider
    {
        public QueryTreeData Provide(JObject jObject)
        {
            QueryTreeData result = new QueryTreeData();
            result.QueryCommandType = ParseCommandType(jObject);
            return result;
        }

        private QueryCommandType ParseCommandType(JObject jObject)
        {
            var cmdType = jObject.SelectToken("QUERY.commandType");
            return EnumParsingSupport.ConvertFromNumericOrDefault<QueryCommandType>(cmdType.Value<int>());
        }
    }
}
