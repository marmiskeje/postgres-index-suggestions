using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class AttributeHPartitioningDesigner : IAttributeHPartitioningDesigner
    {
        private const int MIN_COUNTS_OF_PARTITIONS = 3;
        private const int MAX_COUNT_OF_PARTITIONS = 100;
        private const int POSSIBLE_ADDITIONAL_PARTITIONS_COUNT = 2;
        private static readonly HashSet<string> comparableOperators = new HashSet<string>(new string[] { ">", ">=", "<", "<=" });
        public HPartitioningAttributeDefinition DesignFor(AttributeData attribute, ISet<string> operators, PrimaryKeyData relationPrimaryKey)
        {
            // if relation has primary key, attribute must be part of it
            if (relationPrimaryKey == null || relationPrimaryKey.Attributes.Select(x => x.Name).ToHashSet().Contains(attribute.Name))
            {
                // range partitioning only for NotNull numeric/datetime attributes
                if (IsTypeSuitableForRange(attribute.IsNullable, attribute.DbType))
                {
                    switch (attribute.DbType)
                    {
                        case DbType.Byte:
                        case DbType.Decimal:
                        case DbType.Double:
                        case DbType.Int16:
                        case DbType.Int32:
                        case DbType.Int64:
                        case DbType.SByte:
                        case DbType.Single:
                        case DbType.UInt16:
                        case DbType.UInt32:
                        case DbType.UInt64:
                        case DbType.VarNumeric:
                        case DbType.Time:
                        case DbType.Date:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                            return DesignRange(attribute);
                    }
                }
                // hash partitioning only for attributes where user never applied comparison operators 
                else if (comparableOperators.Intersect(operators).Count() == 0) // hash
                {
                    switch (attribute.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                        case DbType.Boolean:
                        case DbType.Byte:
                        case DbType.Currency:
                        case DbType.Date:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                        case DbType.Decimal:
                        case DbType.Double:
                        case DbType.Guid:
                        case DbType.Int16:
                        case DbType.Int32:
                        case DbType.Int64:
                        case DbType.SByte:
                        case DbType.Single:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Time:
                        case DbType.UInt16:
                        case DbType.UInt32:
                        case DbType.UInt64:
                        case DbType.VarNumeric:
                            return DesignHash(attribute);
                    }
                }
            }
            return null;
        }

        private bool IsTypeSuitableForRange(bool isNullable, DbType type)
        {
            if (isNullable)
            {
                switch (type)
                {
                    case DbType.Byte:
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                    case DbType.Decimal:
                    case DbType.Double:
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.Int64:
                    case DbType.SByte:
                    case DbType.Single:
                    case DbType.Time:
                    case DbType.UInt16:
                    case DbType.UInt32:
                    case DbType.UInt64:
                    case DbType.VarNumeric:
                        return true;
                } 
            }
            return false;
        }

        private HPartitioningAttributeDefinition DesignRange(AttributeData attribute)
        {
            RangeHPartitioningAttributeDefinition result = null;
            var histogramBounds = new List<object>(attribute.HistogramBounds ?? new object[0]);
            if (histogramBounds.Count >= MIN_COUNTS_OF_PARTITIONS)
            {
                result = new RangeHPartitioningAttributeDefinition(attribute);
                var additionalPartitionsCount = Math.Min(Math.Max(MAX_COUNT_OF_PARTITIONS - histogramBounds.Count, 0), POSSIBLE_ADDITIONAL_PARTITIONS_COUNT);
                int partitionsCount = Math.Max(histogramBounds.Count + additionalPartitionsCount, MAX_COUNT_OF_PARTITIONS);
                int takeCount = Math.Max((int)Math.Ceiling(histogramBounds.Count / (double)partitionsCount), 1);
                int counter = 0;
                while (counter < partitionsCount)
                {
                    var bounds = attribute.HistogramBounds.Skip(counter).Take(takeCount);
                    var startBoundary = bounds.First();
                    if (counter > 0)
                    {
                        startBoundary = histogramBounds[counter - 1];
                    }
                    var endBoundary = bounds.Last();
                    result.Partitions.Add(new RangeHPartitionAttributeDefinition(attribute.DbType, startBoundary, endBoundary));
                    counter += takeCount;
                }
                if (result.Partitions.Count < MAX_COUNT_OF_PARTITIONS)
                {
                    int anotherAdditionalPartitionsCount = Math.Min(MAX_COUNT_OF_PARTITIONS - result.Partitions.Count, POSSIBLE_ADDITIONAL_PARTITIONS_COUNT);
                    object startBoundary = result.Partitions.Last().ToValueExclusive;
                    object endBoundary = GetNextEndBoundary(attribute.DbType, result.Partitions.Last().FromValueInclusive, result.Partitions.Last().ToValueExclusive);
                    int additionalPartitionsCounter = 0;
                    while (endBoundary != null && additionalPartitionsCounter < anotherAdditionalPartitionsCount && IsMaxValue(attribute.DbType, endBoundary))
                    {
                        result.Partitions.Add(new RangeHPartitionAttributeDefinition(attribute.DbType, startBoundary, endBoundary));
                        additionalPartitionsCounter++;
                        var previousEndBoundary = endBoundary;
                        endBoundary = GetNextEndBoundary(attribute.DbType, startBoundary, previousEndBoundary);
                        startBoundary = previousEndBoundary;
                    }
                }
            }
            return result;
        }

        private object GetNextEndBoundary(DbType type, object previousStartBoundary, object previousEndBoundary)
        {
            try
            {
                switch (type)
                {
                    case DbType.Byte:
                        return ((byte)previousEndBoundary * 2) - (byte)previousStartBoundary;
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        return new DateTime(((DateTime)previousEndBoundary).Ticks * 2 - ((DateTime)previousStartBoundary).Ticks);
                    case DbType.Decimal:
                        return ((decimal)previousEndBoundary * 2) - (decimal)previousStartBoundary;
                    case DbType.Double:
                        return ((double)previousEndBoundary * 2) - (double)previousStartBoundary;
                    case DbType.Int16:
                        return ((Int16)previousEndBoundary * 2) - (Int16)previousStartBoundary;
                    case DbType.Int32:
                        return ((Int32)previousEndBoundary * 2) - (Int32)previousStartBoundary;
                    case DbType.Int64:
                        return ((Int64)previousEndBoundary * 2) - (Int64)previousStartBoundary;
                    case DbType.SByte:
                        return ((sbyte)previousEndBoundary * 2) - (sbyte)previousStartBoundary;
                    case DbType.Single:
                        return ((float)previousEndBoundary * 2) - (float)previousStartBoundary;
                    case DbType.Time:
                        return new TimeSpan(((TimeSpan)previousEndBoundary).Ticks * 2 - ((TimeSpan)previousStartBoundary).Ticks);
                    case DbType.UInt16:
                        return ((UInt16)previousEndBoundary * 2) - (UInt16)previousStartBoundary;
                    case DbType.UInt32:
                        return ((UInt32)previousEndBoundary * 2) - (UInt32)previousStartBoundary;
                    case DbType.UInt64:
                        return ((UInt64)previousEndBoundary * 2) - (UInt64)previousStartBoundary;
                }
            }
            catch (OverflowException)
            {
                // ignore
            }
            return null;
        }

        private bool IsMaxValue(DbType type, object obj)
        {
            switch (type)
            {
                case DbType.Byte:
                    return byte.MaxValue == (byte)obj;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return DateTime.MaxValue.Date == ((DateTime)obj).Date;
                case DbType.Decimal:
                    return decimal.MaxValue == (decimal)obj;
                case DbType.Double:
                    return double.MaxValue == (double)obj;
                case DbType.Int16:
                    return Int16.MaxValue == (Int16)obj;
                case DbType.Int32:
                    return Int32.MaxValue == (Int32)obj;
                case DbType.Int64:
                    return Int64.MaxValue == (Int64)obj;
                case DbType.SByte:
                    return sbyte.MaxValue == (sbyte)obj;
                case DbType.Single:
                    return float.MaxValue == (float)obj;
                case DbType.Time:
                    return TimeSpan.MaxValue.TotalSeconds == ((TimeSpan)obj).TotalSeconds;
                case DbType.UInt16:
                    return UInt16.MaxValue == (UInt16)obj;
                case DbType.UInt32:
                    return UInt32.MaxValue == (UInt32)obj;
                case DbType.UInt64:
                    return UInt64.MaxValue == (UInt64)obj;
            }
            throw new NotSupportedException(type.ToString());
        }

        private HPartitioningAttributeDefinition DesignHash(AttributeData attribute)
        {
            HashHPartitioningAttributeDefinition result = null;
            if (attribute.HistogramBounds != null && attribute.HistogramBounds.Length >= MIN_COUNTS_OF_PARTITIONS)
            {
                var additionalPartitionsCount = Math.Min(Math.Max(MAX_COUNT_OF_PARTITIONS - attribute.HistogramBounds.Length, 0), POSSIBLE_ADDITIONAL_PARTITIONS_COUNT);
                int partitionsCount = Math.Max(attribute.HistogramBounds.Length + additionalPartitionsCount, MAX_COUNT_OF_PARTITIONS);
                result = new HashHPartitioningAttributeDefinition(attribute, partitionsCount);
            }
            return result;
        }
    }
}
