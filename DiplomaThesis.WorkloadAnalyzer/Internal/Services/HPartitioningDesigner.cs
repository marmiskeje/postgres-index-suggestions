using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class AttributeHPartitioningDesigner : IAttributeHPartitioningDesigner
    {
        private const int MIN_COUNTS_OF_PARTITIONS = 2;
        private const int MAX_COUNT_OF_PARTITIONS = 50;
        private const int POSSIBLE_ADDITIONAL_PARTITIONS_COUNT = 2;
        private static readonly HashSet<string> comparableOperators = new HashSet<string>(new string[] { ">", ">=", "<", "<=" });
        private readonly bool supportsHashPartitioning = false;
        public AttributeHPartitioningDesigner(bool supportsHashPartitioning)
        {
            this.supportsHashPartitioning = supportsHashPartitioning;
        }
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
                else if (supportsHashPartitioning && comparableOperators.Intersect(operators).Count() == 0) // hash
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
            if (!isNullable)
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
            List<string> histogramBounds = new List<string>();
            if (attribute.HistogramBounds != null && attribute.HistogramBounds.Length > 0)
            {
                histogramBounds.AddRange(attribute.HistogramBounds);
            }
            else if (attribute.MostCommonValues != null && attribute.MostCommonValues.Length > 0)
            {
                histogramBounds.AddRange(attribute.MostCommonValues);
            }
            histogramBounds.Sort(GetComparer(attribute.DbType));
            if (histogramBounds.Count > MIN_COUNTS_OF_PARTITIONS)
            {
                result = new RangeHPartitioningAttributeDefinition(attribute);
                int partitionsCount = Math.Min(histogramBounds.Count - 1, MAX_COUNT_OF_PARTITIONS);
                int takeCount = Math.Max((int)Math.Ceiling(histogramBounds.Count / (double)partitionsCount), 1);
                int counter = 0;
                string startBoundary = histogramBounds.First();
                while (counter < partitionsCount)
                {
                    var bounds = histogramBounds.Skip(counter).Take(takeCount);
                    string endBoundary = bounds.Last();
                    result.Partitions.Add(new RangeHPartitionAttributeDefinition(attribute.DbType, startBoundary, endBoundary));
                    counter++;
                    startBoundary = endBoundary;
                }
                if (result.Partitions.Count < MAX_COUNT_OF_PARTITIONS)
                {
                    int anotherAdditionalPartitionsCount = Math.Min(MAX_COUNT_OF_PARTITIONS - result.Partitions.Count, POSSIBLE_ADDITIONAL_PARTITIONS_COUNT);
                    startBoundary = result.Partitions.Last().ToValueExclusive;
                    string endBoundary = GetNextEndBoundary(attribute.DbType, result.Partitions.Last().FromValueInclusive, result.Partitions.Last().ToValueExclusive);
                    int additionalPartitionsCounter = 0;
                    while (endBoundary != null && additionalPartitionsCounter < anotherAdditionalPartitionsCount && !IsMaxValue(attribute.DbType, endBoundary))
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

        private class NumericValueComparer : IComparer<string>
        {
            private static readonly CultureInfo cultureInfo = new CultureInfo("en-US");
            public int Compare(string x, string y)
            {
                return decimal.Parse(x, cultureInfo).CompareTo(decimal.Parse(y, cultureInfo));
            }
        }

        private class DateTimeValueComparer : IComparer<string>
        {
            private static readonly CultureInfo cultureInfo = new CultureInfo("en-US");
            public int Compare(string x, string y)
            {
                return DateTime.Parse(x).CompareTo(DateTime.Parse(y));
            }
        }

        private class TimeSpanValueComparer : IComparer<string>
        {
            private static readonly CultureInfo cultureInfo = new CultureInfo("en-US");
            public int Compare(string x, string y)
            {
                return TimeSpan.Parse(x).CompareTo(TimeSpan.Parse(y));
            }
        }

        private IComparer<string> GetComparer(DbType type)
        {
            switch (type)
            {
                case DbType.Byte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return new NumericValueComparer();
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                    return new NumericValueComparer();
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return new DateTimeValueComparer();
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                    return new NumericValueComparer();
                case DbType.Time:
                    return new TimeSpanValueComparer();
            }
            return null;
        }

        private string GetNextEndBoundary(DbType type, string previousStartBoundary, string previousEndBoundary)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            try
            {
                switch (type)
                {
                    case DbType.Byte:
                    case DbType.UInt16:
                    case DbType.UInt32:
                    case DbType.UInt64:
                        {
                            var end = UInt64.Parse(previousEndBoundary);
                            var start = UInt64.Parse(previousStartBoundary);
                            return $"{end + (end - start)}";
                        }
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.Int64:
                    case DbType.SByte:
                        {
                            var end = Int64.Parse(previousEndBoundary);
                            var start = Int64.Parse(previousStartBoundary);
                            return $"{end + (end - start)}";
                        }
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        {
                            var end = DateTime.Parse(previousEndBoundary, cultureInfo);
                            var start = DateTime.Parse(previousStartBoundary, cultureInfo);
                            return $"{new DateTime(end.Ticks + (end.Ticks - start.Ticks)).ToString("yyyy-MM-dd HH:mm:ss.fff")}";
                        }
                    case DbType.Decimal:
                    case DbType.Double:
                    case DbType.Single:
                        {
                            var end = Decimal.Parse(previousEndBoundary, cultureInfo);
                            var start = Decimal.Parse(previousStartBoundary, cultureInfo);
                            return $"{end + (end - start)}";
                        }
                    case DbType.Time:
                        {
                            var end = TimeSpan.Parse(previousEndBoundary, cultureInfo);
                            var start = TimeSpan.Parse(previousStartBoundary, cultureInfo);
                            return $"{(end + (end - start)).ToString("HH:mm:ss.fff")}";
                        }
                }
            }
            catch (OverflowException)
            {
                // ignore
            }
            return null;
        }

        private bool IsMaxValue(DbType type, string obj)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            switch (type)
            {
                case DbType.Byte:
                    return byte.MaxValue == byte.Parse(obj);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return DateTime.MaxValue.Date == (DateTime.Parse(obj, cultureInfo)).Date;
                case DbType.Decimal:
                    return decimal.MaxValue == decimal.Parse(obj);
                case DbType.Double:
                    return double.MaxValue == double.Parse(obj);
                case DbType.Int16:
                    return Int16.MaxValue == Int16.Parse(obj);
                case DbType.Int32:
                    return Int32.MaxValue == Int32.Parse(obj);
                case DbType.Int64:
                    return Int64.MaxValue == Int64.Parse(obj);
                case DbType.SByte:
                    return sbyte.MaxValue == sbyte.Parse(obj);
                case DbType.Single:
                    return float.MaxValue == float.Parse(obj);
                case DbType.Time:
                    return TimeSpan.MaxValue.TotalSeconds == (TimeSpan.Parse(obj, cultureInfo)).TotalSeconds;
                case DbType.UInt16:
                    return UInt16.MaxValue == UInt16.Parse(obj);
                case DbType.UInt32:
                    return UInt32.MaxValue == UInt32.Parse(obj);
                case DbType.UInt64:
                    return UInt64.MaxValue == UInt64.Parse(obj);
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
