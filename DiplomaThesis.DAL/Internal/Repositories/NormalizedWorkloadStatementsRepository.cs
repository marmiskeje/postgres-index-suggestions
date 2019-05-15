using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class NormalizedWorkloadStatementsRepository : BaseSimpleRepository<NormalizedWorkloadStatement>, INormalizedWorkloadStatementsRepository
    {
        public NormalizedWorkloadStatementsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IEnumerable<NormalizedWorkloadStatement> GetWorkloadStatements(Workload workload, DateTime fromInclusive, DateTime toExclusive)
        {
            using (var context = CreateContextFunc())
            {
                var query = context.NormalizedStatementStatistics
                    .Where(x => x.DatabaseID == workload.DatabaseID)
                    .Where(x => x.Date >= fromInclusive)
                    .Where(x => x.Date < toExclusive);
                query = query.Where(x => x.NormalizedStatement.CommandType == StatementQueryCommandType.Delete
                                    || x.NormalizedStatement.CommandType == StatementQueryCommandType.Insert
                                    || x.NormalizedStatement.CommandType == StatementQueryCommandType.Update
                                    || x.NormalizedStatement.CommandType == StatementQueryCommandType.Select);
                if (workload.Definition.Applications.ForbiddenValues.Count > 0)
                {
                    query = query.Where(x => !workload.Definition.Applications.ForbiddenValues.Contains(x.ApplicationName));
                }
                if (workload.Definition.Users.ForbiddenValues.Count > 0)
                {
                    query = query.Where(x => !workload.Definition.Users.ForbiddenValues.Contains(x.UserName));
                }
                if (workload.Definition.QueryThresholds.MinDuration.HasValue)
                {
                    query = query.Where(x => x.MaxDuration >= workload.Definition.QueryThresholds.MinDuration.Value
                        || x.MinDuration >= workload.Definition.QueryThresholds.MinDuration.Value
                        || x.AvgDuration >= workload.Definition.QueryThresholds.MinDuration.Value);
                }
                var groupedQuery = from item in query
                        group item by item.NormalizedStatementID into g
                        select new { Key = g.Key, Items = g };
                if (workload.Definition.QueryThresholds.MinExectutionCount.HasValue)
                {
                    groupedQuery = from item in groupedQuery
                                   where item.Items.Sum(x => x.TotalExecutionsCount) > workload.Definition.QueryThresholds.MinExectutionCount
                                   select new { Key = item.Key, Items = item.Items };
                }
                var groupedResult = groupedQuery.ToDictionary(x => x.Key, x => x.Items.ToHashSet());
                var reducedGroupedResult = new Dictionary<(long NormalizedStatementID, long TotalExecutionsCount), NormalizedStatementStatistics>();
                foreach (var item in groupedResult)
                {
                    var values = item.Value.Where(stat => (workload.Definition.DateTimeSlots.ForbiddenValues.FirstOrDefault(x => x.DayOfWeek == stat.Date.DayOfWeek
                                    && x.StartTime >= stat.Date.TimeOfDay && stat.Date.TimeOfDay <= x.EndTime) == null));
                    reducedGroupedResult.Add((item.Key, values.Sum(x => x.TotalExecutionsCount)), values.OrderByDescending(x => x.MaxDuration).FirstOrDefault());
                }
                var result = from item in reducedGroupedResult
                             join statement in context.NormalizedStatements on item.Key.NormalizedStatementID equals statement.ID
                             where statement.StatementDefinitionData != null
                             select new NormalizedWorkloadStatement() { NormalizedStatement = statement, RepresentativeStatistics = item.Value, TotalExecutionsCount = item.Key.TotalExecutionsCount };
                foreach (var item in result)
                {
                    NormalizedStatementsRepository.FillEntityForGet(item.NormalizedStatement);
                }
                return result.ToList();
            }
        }
    }
}
