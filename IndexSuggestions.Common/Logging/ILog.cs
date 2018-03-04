using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.Logging
{
    public interface ILog
    {
        void Write(Exception ex);
        void Write(SeverityType severity, string messageOrFormat, params object[] args);
    }

    [Serializable]
    public enum SeverityType
    {
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3,
        Failure = 4,
        Recovery = 5,
        Success = 6
    }
}
