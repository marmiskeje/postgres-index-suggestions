using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.Logging.NLog
{
    public class NLog : ILog
    {
        private readonly Logger logger;
        private static readonly Lazy<ILog> instance = new Lazy<ILog>(() => new NLog()); // threadsafe

        public static ILog Instace
        {
            get { return instance.Value; }
        }

        private NLog()
        {
            logger = LogManager.GetLogger("Logger");
        }
        public void Write(Exception ex)
        {
            logger.Log(LogLevel.Error, ex, ex.Message);
        }

        public void Write(SeverityType severity, string messageOrFormat, params object[] args)
        {
            logger.Log(Convert(severity), String.Format(messageOrFormat, args));
        }

        private LogLevel Convert(SeverityType severity)
        {
            switch (severity)
            {
                case SeverityType.Debug:
                    return LogLevel.Debug;
                case SeverityType.Warning:
                    return LogLevel.Warn;
                case SeverityType.Error:
                    return LogLevel.Error;
                case SeverityType.Failure:
                    return LogLevel.Fatal;
                case SeverityType.Info:
                case SeverityType.Recovery:
                case SeverityType.Success:
                default:
                    return LogLevel.Info;
            }
        }
    }
}
