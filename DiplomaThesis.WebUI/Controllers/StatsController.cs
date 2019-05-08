
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : BaseApiController
    {
        [Route("overview")]
        [HttpPost]
        public IActionResult GetOverviewStats(StatsOverviewRequest request)
        {
            StatsOverviewResponse result = new StatsOverviewResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new StatsOverviewData();
                    int topCount = 10;
                    var totalRelationStatsRepository = DALRepositories.GetTotalRelationStatisticsRepository();
                    var statementStatsRepository = DALRepositories.GetNormalizedStatementStatisticsRepository();
                    // alive relations
                    var summaryTotalRelationStatistics = totalRelationStatsRepository.GetSummaryTotalRelationStatistics(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, topCount);
                    data.MostAliveRelations = Converter.ConvertToMostAliveRelations(summaryTotalRelationStatistics);
                    // top executed
                    var topExecuted = statementStatsRepository.GetSummaryTotalStatementStatistics(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, DAL.Contracts.SummaryNormalizedStatementStatisticsOrderBy.ExecutionCount, null, topCount);
                    data.MostExecutedStatements = Converter.ConvertToMostExecutedStatements(topExecuted);
                    // slowest
                    var topSlowest = statementStatsRepository.GetSummaryTotalStatementStatistics(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, DAL.Contracts.SummaryNormalizedStatementStatisticsOrderBy.MaxDuration, null, topCount);
                    data.MostSlowestStatements = Converter.ConvertToMostSlowestStatements(topSlowest);
                    result.Data = data;
                    result.IsSuccess = data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("relation")]
        [HttpPost]
        public IActionResult GetRelationStatistics(GetRelationStatisticsRequest request)
        {
            GetRelationStatisticsResponse result = new GetRelationStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new RelationStatisticsData();
                    var totalRelationStatsRepository = DALRepositories.GetTotalRelationStatisticsRepository();
                    var statementRelationStatisticsRepository = DALRepositories.GetNormalizedStatementRelationStatisticsRepository();
                    data.TotalRelationStatistics = totalRelationStatsRepository.GetAllForRelation(request.RelationID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    data.RelationSummaryStatementStatistics = statementRelationStatisticsRepository.GetRelationSummaryStatementStatistics(request.RelationID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    result.Data = data;
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("index")]
        [HttpPost]
        public IActionResult GetIndexStatistics(GetIndexStatisticsRequest request)
        {
            GetIndexStatisticsResponse result = new GetIndexStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new IndexStatisticsData();
                    var totalIndexStatsRepository = DALRepositories.GetTotalIndexStatisticsRepository();
                    var statementIndexStatisticsRepository = DALRepositories.GetNormalizedStatementIndexStatisticsRepository();
                    data.TotalIndexStatistics = totalIndexStatsRepository.GetAllForIndex(request.IndexID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    data.IndexSummaryStatementStatistics = statementIndexStatisticsRepository.GetIndexSummaryStatementStatistics(request.IndexID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    result.Data = data;
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("stored-procedure")]
        [HttpPost]
        public IActionResult GetStoredProcedureStatistics(GetStoredProcedureStatisticsRequest request)
        {
            GetStoredProcedureStatisticsResponse result = new GetStoredProcedureStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new StoredProcedureStatisticsData();
                    var totalStoredProcStatsRepository = DALRepositories.GetTotalStoredProcedureStatisticsRepository();
                    data.TotalStoredProcedureStatistics = totalStoredProcStatsRepository.GetAllForProcedure(request.StoredProcedureID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    result.Data = data;
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("statements")]
        [HttpPost]
        public IActionResult GetStatementsStatistics(GetStatementsStatisticsRequest request)
        {
            GetStatementsStatisticsResponse result = new GetStatementsStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new StatementsStatisticsData();
                    var normalizedStatementStatisticsRepository = DALRepositories.GetNormalizedStatementStatisticsRepository();
                    data.SummaryNormalizedStatementStatistics = normalizedStatementStatisticsRepository.GetSummaryTotalStatementStatistics(request.DatabaseID,
                        request.Filter.DateFrom, request.Filter.DateTo, DAL.Contracts.SummaryNormalizedStatementStatisticsOrderBy.MaxDuration,
                        Converter.CreateCommandTypeFilter(request.Filter.CommandType), null).ToList();
                    result.Data = data;
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("statement")]
        [HttpPost]
        public IActionResult GetStatementStatistics(GetStatementStatisticsRequest request)
        {
            GetStatementStatisticsResponse result = new GetStatementStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var data = new StatementStatisticsData();
                    var totalStatsRepository = DALRepositories.GetNormalizedStatementStatisticsRepository();
                    var relationStatsRepository = DALRepositories.GetNormalizedStatementRelationStatisticsRepository();
                    var indexStatsRepository = DALRepositories.GetNormalizedStatementIndexStatisticsRepository();
                    data.StatementTimeline = totalStatsRepository.GetTimelineForStatement(request.StatementID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    data.StatementRelationStatistics = relationStatsRepository.GetAllForStatement(request.StatementID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    data.StatementIndexStatistics = indexStatsRepository.GetAllForStatement(request.StatementID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    data.SlowestRepresentatives = totalStatsRepository.GetSlowestForStatement(request.StatementID, request.Filter.DateFrom, request.Filter.DateTo, 10).ToList();
                    result.Data = data;
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
    }
}
