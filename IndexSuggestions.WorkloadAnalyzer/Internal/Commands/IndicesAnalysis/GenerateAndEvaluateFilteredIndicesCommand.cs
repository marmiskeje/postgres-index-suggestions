using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class GenerateAndEvaluateFilteredIndicesCommand : ChainableCommand
    {
        private const int MOST_COMMON_VALUES_MAX_COUNT = 6;
        private const decimal MOST_COMMON_VALUE_MIN_FREQUENCY = 10m;
        private const decimal MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM = 15m;
        private readonly WorkloadAnalysisContext context;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        private readonly IVirtualIndicesRepository virtualIndicesRepository;
        private readonly ISqlCreateStatementGenerator sqlCreateStatementGenerator;
        public GenerateAndEvaluateFilteredIndicesCommand(WorkloadAnalysisContext context, IToSqlValueStringConverter toSqlValueStringConverter,
                                                              IVirtualIndicesRepository virtualIndicesRepository, ISqlCreateStatementGenerator sqlCreateStatementGenerator)
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
                    Dictionary<AttributeData, List<string>> possibleFilteredAttributeValues = new Dictionary<AttributeData, List<string>>();
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
                        var targetRelationData = context.RelationsData.GetReplacementOrOriginal(index.Relation.ID);
                        var virtualIndex = virtualIndicesRepository.Create(sqlCreateStatementGenerator.Generate(index.WithReplacedRelation(targetRelationData), filter));
                        var size = virtualIndicesRepository.GetVirtualIndexSize(virtualIndex.ID);
                        var filters = new Dictionary<string, long>();
                        filters.Add(filter, size);
                        context.IndicesDesignData.PossibleIndexFilters.Add(index, filters);
                    }
                } 
            }
        }

        private string CreateFilterString(Dictionary<AttributeData, List<string>> possibleFilteredAttributeValues)
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
