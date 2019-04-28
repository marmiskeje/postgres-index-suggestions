using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.ReportingService
{
    internal abstract class ReportContext
    {
        public DateTime DateFromInclusive { get; set; }
        public DateTime DateToExclusive { get; set; }
        public EmailDefinition EmailDefinition { get; set; }
    }

    internal class ReportContextWithModel<TModel> : ReportContext
    {
        public string TemplateId { get; set; }
        public TModel Model { get; set; }
    }

    internal class EmailDefinition
    {
        public string Sender { get; set; }
        public HashSet<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
