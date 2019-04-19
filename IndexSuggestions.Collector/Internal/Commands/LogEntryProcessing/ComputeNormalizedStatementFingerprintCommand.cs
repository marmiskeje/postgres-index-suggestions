using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class ComputeNormalizedStatementFingerprintCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        public ComputeNormalizedStatementFingerprintCommand(LogEntryProcessingContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            using (SHA512 sha = new SHA512Managed())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(context.StatementData.NormalizedStatement));
                context.StatementData.NormalizedStatementFingerprint = Convert.ToBase64String(hash);
            }
        }
    }
}
