using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class GenerateFilteredBtreeIndicesCommand : ChainableCommand
    {
        private const int MOST_COMMON_VALUES_MAX_COUNT = 6;
        private const decimal MOST_COMMON_VALUE_MIN_FREQUENCY = 10m;
        private const decimal MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM = 15m;
        private readonly DesignIndicesContext context;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        public GenerateFilteredBtreeIndicesCommand(DesignIndicesContext context, IToSqlValueStringConverter toSqlValueStringConverter)
        {
            this.context = context;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
        }
        protected override void OnExecute()
        {
            foreach (var index in context.IndicesDesignData.PossibleIndices.All)
            {
                Dictionary<IndexAttribute, List<string>> possibleFilteredAttributeValues = new Dictionary<IndexAttribute, List<string>>();
                foreach (var a in index.Attributes)
                {
                    List<string> mostSignificantValues = new List<string>();
                    if (a.MostCommonValuesFrequencies != null)
                    {
                        decimal frequenciesSum = 0;
                        for (int i = 0; i < Math.Min(a.MostCommonValuesFrequencies.Length, MOST_COMMON_VALUES_MAX_COUNT); i++)
                        {
                            if (a.MostCommonValuesFrequencies[i] >= MOST_COMMON_VALUE_MIN_FREQUENCY)
                            {
                                var sqlStringValue = toSqlValueStringConverter.Convert(a.DbType, a.MostCommonValues[i]);
                                if (sqlStringValue != null)
                                {
                                    mostSignificantValues.Add(sqlStringValue);
                                    frequenciesSum += a.MostCommonValuesFrequencies[i];
                                }
                            }
                        }
                        if (frequenciesSum < MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM)
                        {
                            mostSignificantValues.Clear();
                        }
                    }
                    if (mostSignificantValues.Count > 0)
                    {
                        possibleFilteredAttributeValues.Add(a, mostSignificantValues);
                    }
                }
                if (possibleFilteredAttributeValues.Count > 0)
                {
                    string filter = CreateFilterString(possibleFilteredAttributeValues);
                    context.IndicesDesignData.PossibleIndexFilters.Add(index, new HashSet<string>(new[] { filter }));
                }
            }
        }

        private string CreateFilterString(Dictionary<IndexAttribute, List<string>> possibleFilteredAttributeValues)
        {
            StringBuilder builder = new StringBuilder();
            int attributesCounter = 0;
            foreach (var kv in possibleFilteredAttributeValues)
            {
                var attribute = kv.Key;
                var values = kv.Value;
                if (attributesCounter > 0)
                {
                    builder.Append("AND ");
                }
                builder.Append("(");
                int valuesCounter = 0;
                foreach (var val in values)
                {
                    if (valuesCounter > 0)
                    {
                        builder.Append("OR ");
                    }
                    builder.Append($"{attribute.Name} = {val}");
                    valuesCounter++;
                }
                builder.Append(")");
                attributesCounter++;
            }
            return builder.ToString();
        }
    }
}
