using gudusoft.gsqlparser;
using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DiplomaThesis.Collector.Postgres
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
            string result = null;
            result = context.Entry.Statement;
            TGSqlParser parser = new TGSqlParser(EDbVendor.dbvpostgresql);
            parser.sqltext = context.Entry.Statement;
            int counter = 0;
            if (parser.parse() == 0)
            {
                foreach (TCustomSqlStatement s in parser.sqlstatements)
                {
                    foreach (TSourceToken t in s.sourcetokenlist)
                    {
                        if (t.tokentype == ETokenType.ttnumber || t.tokentype == ETokenType.ttsqstring)
                        {
                            counter++;
                            t.astext = @"$" + counter.ToString();
                        }
                    }
                    result = s.String;
                    break;
                }
            }
            else
            {
                log.Write(SeverityType.Error, parser.Errormessage);
                this.IsEnabledSuccessorCall = false;
            }
            if (!String.IsNullOrEmpty(result))
            {
                context.StatementData.NormalizedStatement = result.Trim().ToUpper();
            }
        }
    }
}
