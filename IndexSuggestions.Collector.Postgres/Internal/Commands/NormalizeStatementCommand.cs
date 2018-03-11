using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    public class NormalizeStatementCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly LogEntryProcessingContext context;
        public NormalizeStatementCommand(ILog log, LogEntryProcessingContext context)
        {
            this.log = log;
            this.context = context;
        }
        protected override void OnExecute()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts/NormalizeStatement.rb");
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo("ruby") { Arguments = file, UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true };
                    process.Start();
                    process.StandardInput.WriteLine(context.Entry.Statement);
                    context.NormalizedStatement = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(5000);
                    var error = process.StandardError.ReadLine();
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                    if (process.ExitCode != 0)
                    {
                        log.Write(SeverityType.Error, error);
                        this.IsEnabledSuccessorCall = false;
                    }
                }
            }
            else
            {
                context.NormalizedStatement = context.Entry.Statement;
            }
        }
    }
}
