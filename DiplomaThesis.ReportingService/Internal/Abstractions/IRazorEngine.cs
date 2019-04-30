using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.ReportingService
{
    public interface IRazorEngine : IDisposable
    {
        string Transform<T>(string template, T model);
    }
}
