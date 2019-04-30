using DiplomaThesis.DAL.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL
{
    internal class TotalStoredProcedureStatisticsRepository : BaseRepository<long, TotalStoredProcedureStatistics>, ITotalStoredProcedureStatisticsRepository
    {
        public TotalStoredProcedureStatisticsRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {
            
        }

        public IReadOnlyDictionary<uint, List<TotalStoredProcedureStatistics>> GetAllGroupedByProcedure(DateTime createdFrom, DateTime createdTo)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics
                    .Where(x => x.CreatedDate >= createdFrom && x.CreatedDate <= createdTo)
                    .GroupBy(x => x.ProcedureID)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public TotalStoredProcedureStatistics GetByUniqueKey(TotalStoredProcedureStatisticsUniqueKey key)
        {
            using (var context = CreateContextFunc())
            {
                return context.TotalStoredProcedureStatistics.Where(x => x.DatabaseID == key.DatabaseID && x.Date == key.Date && x.ProcedureID == key.ProcedureID).SingleOrDefault();
            }
        }
    }
}
