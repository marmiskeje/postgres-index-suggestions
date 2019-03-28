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
}
