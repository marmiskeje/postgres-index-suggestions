using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal abstract class MergeStatisticsContext<TKey, TData>
    {
        public DateTime CreatedDateFrom { get; set; }
        public DateTime CreatedDateTo { get; set; }
        public IDateTimeSelector DateTimeSelector { get; set; }
        public IReadOnlyDictionary<TKey, List<TData>> LoadedStatistics { get; set; }
        public Dictionary<TKey, IReadOnlyCollection<TData>> MergedStatistics { get; } = new Dictionary<TKey, IReadOnlyCollection<TData>>();
    }

    internal class MergeNormalizedStatementStatisticsContext : MergeStatisticsContext<long, NormalizedStatementStatistics>
    {

    }
    internal class MergeNormalizedStatementRelationStatisticsContext : MergeStatisticsContext<long, NormalizedStatementRelationStatistics>
    {

    }
    internal class MergeNormalizedStatementIndexStatisticsContext : MergeStatisticsContext<long, NormalizedStatementIndexStatistics>
    {

    }

    internal class MergeTotalRelationStatisticsContext : MergeStatisticsContext<uint, TotalRelationStatistics>
    {

    }
    internal class MergeTotalIndexStatisticsContext : MergeStatisticsContext<uint, TotalIndexStatistics>
    {

    }
    internal class MergeTotalStoredProcedureStatisticsContext : MergeStatisticsContext<uint, TotalStoredProcedureStatistics>
    {

    }
    internal class MergeTotalViewStatisticsContext : MergeStatisticsContext<uint, TotalViewStatistics>
    {

    }
}
