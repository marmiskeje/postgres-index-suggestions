using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface IRepositoriesFactory
    {
        INormalizedStatementsRepository GetNormalizedStatementsRepository();
        ISettingPropertiesRepository GetSettingPropertiesRepository();
        IWorkloadsRepository GetWorkloadsRepository();
        INormalizedStatementStatisticsRepository GetNormalizedStatementStatisticsRepository();
        INormalizedStatementIndexStatisticsRepository GetNormalizedStatementIndexStatisticsRepository();
        INormalizedStatementRelationStatisticsRepository GetNormalizedStatementRelationStatisticsRepository();
        ITotalRelationStatisticsRepository GetTotalRelationStatisticsRepository();
        ITotalIndexStatisticsRepository GetTotalIndexStatisticsRepository();
        ITotalStoredProcedureStatisticsRepository GetTotalStoredProcedureStatisticsRepository();
        ITotalViewStatisticsRepository GetTotalViewStatisticsRepository();
        INormalizedWorkloadStatementsRepository GetNormalizedWorkloadStatementsRepository();
    }
}
