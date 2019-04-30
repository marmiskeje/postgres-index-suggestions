using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
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
        IWorkloadAnalysesRepository GetWorkloadAnalysesRepository();
        IVirtualEnvironmentsRepository GetVirtualEnvironmentsRepository();
        IPossibleIndicesRepository GetPossibleIndicesRepository();
        IVirtualEnvironmentPossibleIndicesRepository GetVirtualEnvironmentPossibleIndicesRepository();
        IVirtualEnvironmentStatementEvaluationsRepository GetVirtualEnvironmentStatementEvaluationsRepository();
        IExecutionPlansRepository GetExecutionPlansRepository();
        IWorkloadAnalysisRealStatementEvaluationsRepository GetWorkloadAnalysisRealStatementEvaluationsRepository();
        IVirtualEnvironmentPossibleCoveringIndicesRepository GetVirtualEnvironmentPossibleCoveringIndicesRepository();
        IVirtualEnvironmentPossibleHPartitioningsRepository GetVirtualEnvironmentPossibleHPartitioningsRepository();
    }
}
