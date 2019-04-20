using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class PossibleIndex : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CreateDefinition { get; set; }
        [NotMapped]
        public PossibleIndexFilterExpressionsData FilterExpressions { get; set; }
        public string FilterExpressionsData { get; set; }
        [Required]
        public long Size { get; set; }
        public List<VirtualEnvironmentPossibleIndex> VirtualEnvironmentPossibleIndices { get; set; }
        public List<VirtualEnvironmentPossibleCoveringIndex> VirtualEnvironmentPossibleCoveringIndices { get; set; }
    }

    public class PossibleIndexFilterExpressionsData
    {
        public List<PossibleIndexFilterExpression> Expressions { get; set; } = new List<PossibleIndexFilterExpression>();
    }

    public class PossibleIndexFilterExpression
    {
        public string Expression { get; set; }
        public long Size { get; set; }
    }
}
