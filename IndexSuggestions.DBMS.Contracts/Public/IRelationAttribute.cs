using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelationAttribute
    {
        int AttributeNumber { get; }
        uint RelationID { get; }
        string Name { get; }
        uint DbTypeId { get; }
        DbType DbType { get; }
        bool IsNullable { get; }
        IEnumerable<string> SupportedOperators { get; }
        /// <summary>
        /// Less value equals higher cardinality, possible logic:
        /// -1 = unique attribute
        /// Less than 0 = -count of unique values / count of rows (this is for growing count of unique values)
        /// 0 = empty
        /// More than 0 = count of rows / count of unique values (this is for fixed count of unique values)
        /// </summary>
        decimal CardinalityIndicator { get; }
        object[] MostCommonValues { get; }
        decimal[] MostCommonValuesFrequencies { get; }
        object[] HistogramBounds { get; }
    }
}
