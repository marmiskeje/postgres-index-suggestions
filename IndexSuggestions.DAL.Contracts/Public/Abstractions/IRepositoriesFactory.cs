using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface IRepositoriesFactory
    {
        IIndicesRepository GetIndicesRepository();
        INormalizedStatementIndexUsagesRepository GetNormalizedStatementIndexUsagesRepository();
        INormalizedStatementsRepository GetNormalizedStatementsRepository();
        ISettingPropertiesRepository GetSettingPropertiesRepository();
        IWorkloadsRepository GetWorkloadsRepository();
        INormalizedWorkloadStatementsRepository GetNormalizedWorkloadStatementsRepository();
    }
}
