using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class ExplainRepository : BaseRepository, IExplainRepository
    {
        public ExplainRepository(string connectionString) : base(connectionString)
        {

        }
        public IExplainResult Eplain(string statement)
        {
            var statementToUse = statement.Trim();
            string query = @"EXPLAIN (FORMAT JSON) " + statementToUse;
            // todo - fix this, dapper is not usable, it can´t inject parameter for some reason
            return ExecuteQuery<ExplainResult>(query, null).Single();
        }
    }
}
