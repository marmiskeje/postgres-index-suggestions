using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class ExplainRepository : BaseRepository, IExplainRepository
    {
        public ExplainRepository(string connectionString) : base("Server=127.0.0.1;Port=6633;Database=test;User Id=postgres;Password = root;Application Name=IndexSuggestions;") // temporary to virtual machine
        {

        }
        public IExplainResult Eplain(string statement)
        {
            var statementToUse = statement.Trim();
            if (statementToUse.EndsWith(";")) // statement can end with ; and that´s is not acceptable for explain
            {
                statementToUse = statementToUse.Substring(0, statementToUse.Length - 1);
            }
            string query = @"EXPLAIN (FORMAT JSON)(" + statementToUse + ")";
            // todo - fix this, dapper is not usable, it can´t inject parameter for some reason
            return ExecuteQuery<ExplainResult>(query, null).Single();
        }
    }
}
