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
        INormalizedWorkloadStatementsRepository GetNormalizedWorkloadStatementsRepository();
        INormalizedStatementStatisticsRepository GetNormalizedStatementStatisticsRepository();
        INormalizedStatementIndexStatisticsRepository GetNormalizedStatementIndexStatisticsRepository();
    }
}
