using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexSuggestions.WorkloadAnalyzer
{
    /// <summary>
    /// One environment = one possible partitioning (with single attribute, or with two attributes)
    /// </summary>
    internal class GenerateBaseHPartitioningEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public GenerateBaseHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            foreach (var ap in context.HPartitioningDesignData.AttributesPartitioning)
            {
                var attribute = ap.Key;
                context.HPartitioningDesignData.Environments.Clear();
                var partitioning = new HPartitioningDefinition(attribute.Relation);
                partitioning.PartitioningAttributes.Add(ap.Value);
                context.HPartitioningDesignData.Environments.Add(new VirtualHPartitioningEnvironment(partitioning));
            }
            foreach (var relationAttributesPartitionings in context.HPartitioningDesignData.AttributesPartitioning.GroupBy(x => x.Key.Relation))
            {
                var relation = relationAttributesPartitionings.Key;
                foreach (var attributesPartitioningsByType in relationAttributesPartitionings.GroupBy(x => x.Value.GetType()))
                {
                    var attributePartitionings = new ElementSet<HPartitioningAttributeDefinition>(attributesPartitioningsByType.Select(x => x.Value));
                    var combinations = new Combinations<HPartitioningAttributeDefinition>(attributePartitionings, 2);
                    foreach (var combination in combinations)
                    {
                        var partitioning = new HPartitioningDefinition(relation);
                        partitioning.PartitioningAttributes.AddRange(combination);
                        context.HPartitioningDesignData.Environments.Add(new VirtualHPartitioningEnvironment(partitioning));
                    }
                }
            }
        }
    }
}
