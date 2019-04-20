using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class GenerateAndEvaluateFilteredBtreeIndicesCommand : ChainableCommand
    {
        private const int MOST_COMMON_VALUES_MAX_COUNT = 6;
        private const decimal MOST_COMMON_VALUE_MIN_FREQUENCY = 10m;
        private const decimal MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM = 15m;
        private readonly WorkloadAnalysisContext context;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        private readonly IVirtualIndicesRepository virtualIndicesRepository;
        private readonly IIndexSqlCreateStatementGenerator sqlCreateStatementGenerator;
        public GenerateAndEvaluateFilteredBtreeIndicesCommand(WorkloadAnalysisContext context, IToSqlValueStringConverter toSqlValueStringConverter,
                                                              IVirtualIndicesRepository virtualIndicesRepository, IIndexSqlCreateStatementGenerator sqlCreateStatementGenerator)
        {
            this.context = context;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
            this.virtualIndicesRepository = virtualIndicesRepository;
            this.sqlCreateStatementGenerator = sqlCreateStatementGenerator;
        }
        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                virtualIndicesRepository.DestroyAll();
                foreach (var index in context.IndicesDesignData.PossibleIndices.All)
                {
                    Dictionary<IndexAttribute, List<string>> possibleFilteredAttributeValues = new Dictionary<IndexAttribute, List<string>>();
                    foreach (var a in index.Attributes)
                    {
                        List<string> mostSignificantValues = new List<string>();
                        if (a.MostCommonValuesFrequencies != null && a.MostCommonValuesFrequencies.Length >= 2)// we need at least two values
                        {
                            decimal frequenciesSum = 0;
                            for (int i = 0; i < Math.Min(a.MostCommonValuesFrequencies.Length - 1, MOST_COMMON_VALUES_MAX_COUNT); i++)
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
                        var virtualIndex = virtualIndicesRepository.Create(new VirtualIndexDefinition() { CreateStatement = sqlCreateStatementGenerator.Generate(index, filter) });
                        var size = virtualIndicesRepository.GetVirtualIndexSize(virtualIndex.ID);
                        var filters = new Dictionary<string, long>();
                        filters.Add(filter, size);
                        context.IndicesDesignData.PossibleIndexFilters.Add(index, filters);
                    }
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
