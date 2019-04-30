using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class EmailTemplate
    {
        public string Subject { get; set; }
        public bool IsBodyHtml { get; set; }
        public string BodyTemplate { get; set; }
    }
}
