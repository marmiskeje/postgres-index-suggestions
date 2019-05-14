using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class GenerateAndEvaluateFilteredIndicesCommand : ChainableCommand
    {
        private const int MOST_COMMON_VALUES_MAX_COUNT = 6;
        private const decimal MOST_COMMON_VALUE_MIN_FREQUENCY = 0.10m; // 10%
        private const decimal MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM = 0.15m; // 15%
        private const decimal MOST_COMMON_VALUES_MAX_FREQUENCIES_SUM = 0.60m; // 60%
        private readonly WorkloadAnalysisContext context;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        private readonly IVirtualIndicesRepository virtualIndicesRepository;
        private readonly IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;
        public GenerateAndEvaluateFilteredIndicesCommand(WorkloadAnalysisContext context, IToSqlValueStringConverter toSqlValueStringConverter,
                                                              IVirtualIndicesRepository virtualIndicesRepository, IDbObjectDefinitionGenerator dbObjectDefinitionGenerator)
        {
            this.context = context;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
            this.virtualIndicesRepository = virtualIndicesRepository;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
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
                        List<string> leastSignificantValues = new List<string>();
                        if (a.MostCommonValuesFrequencies != null && a.MostCommonValuesFrequencies.Length >= 2)// we need at least two values
                        {
                            decimal frequenciesSum = 0;
                            for (int i = 0; i < Math.Min(a.MostCommonValuesFrequencies.Length - 1, MOST_COMMON_VALUES_MAX_COUNT); i++)
                            {
                                if (a.MostCommonValuesFrequencies[i] >= MOST_COMMON_VALUE_MIN_FREQUENCY)
                                {
                                    var sqlStringValue = toSqlValueStringConverter.ConvertStringRepresentation(a.DbType, a.MostCommonValues[i]);
                                    if (sqlStringValue != null)
                                    {
                                        mostSignificantValues.Add(sqlStringValue);
                                        frequenciesSum += a.MostCommonValuesFrequencies[i];
                                    }
                                }
                            }
                            if (frequenciesSum < MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM || frequenciesSum > MOST_COMMON_VALUES_MAX_FREQUENCIES_SUM)
                            {
                                mostSignificantValues.Clear();
                            }

                            frequenciesSum = 0;
                            for (int i = a.MostCommonValuesFrequencies.Length - 1; i >= Math.Max(1, a.MostCommonValuesFrequencies.Length - MOST_COMMON_VALUES_MAX_COUNT + 1); i--)
                            {
                                var sqlStringValue = toSqlValueStringConverter.ConvertStringRepresentation(a.DbType, a.MostCommonValues[i]);
                                if (sqlStringValue != null)
                                {
                                    leastSignificantValues.Add(sqlStringValue);
                                    frequenciesSum += a.MostCommonValuesFrequencies[i];
                                }
                            }
                            if (frequenciesSum > MOST_COMMON_VALUES_MIN_FREQUENCIES_SUM)
                            {
                                leastSignificantValues.Clear();
                            }
                        }
                        if (mostSignificantValues.Count > 0)
                        {
                            possibleFilteredAttributeValues.Add(a, mostSignificantValues);
                        }
                        else if (leastSignificantValues.Count > 0)
                        {
                            possibleFilteredAttributeValues.Add(a, leastSignificantValues);
                        }
                    }
                    if (possibleFilteredAttributeValues.Count > 0)
                    {
                        string filter = CreateFilterString(possibleFilteredAttributeValues);
                        var targetRelationData = context.RelationsData.GetReplacementOrOriginal(index.Relation.ID);
                        var virtualIndex = virtualIndicesRepository.Create(dbObjectDefinitionGenerator.Generate(index.WithReplacedRelation(targetRelationData), filter));
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
                        builder.Append(" OR ");
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
