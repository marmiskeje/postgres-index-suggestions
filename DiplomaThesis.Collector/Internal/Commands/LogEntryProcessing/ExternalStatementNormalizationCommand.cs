using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class ExternalStatementNormalizationCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly IExternalSqlNormalizationConfiguration externalNormalizationConfig;
        private readonly LogEntryProcessingContext context;
        public ExternalStatementNormalizationCommand(ILog log, IExternalSqlNormalizationConfiguration externalNormalizationConfig, LogEntryProcessingContext context)
        {
            this.log = log;
            this.externalNormalizationConfig = externalNormalizationConfig;
            this.context = context;
        }
        protected override void OnExecute()
        {
            string result = null;
            if (externalNormalizationConfig.IsEnabled && !String.IsNullOrEmpty(externalNormalizationConfig.ProcessFilename))
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(externalNormalizationConfig.ProcessFilename)
                    {
                        Arguments = externalNormalizationConfig.ProcessArguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true
                    };
                    process.Start();
                    process.StandardInput.WriteLine(context.Entry.Statement);
                    result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(5000);
                    var error = process.StandardError.ReadLine();
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    if (process.ExitCode != 0)
                    {
                        log.Write(SeverityType.Error, error);
                    }
                }
            }
            if (!String.IsNullOrEmpty(result))
            {
                context.StatementData.NormalizedStatement = result.Trim().ToUpper();
            }
        }
    }
}
