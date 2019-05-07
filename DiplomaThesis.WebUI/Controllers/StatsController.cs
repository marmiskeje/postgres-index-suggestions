
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
                    var topExecuted = statementStatsRepository.GetSummaryTotalStatementStatistics(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, DAL.Contracts.SummaryNormalizedStatementStatisticsOrderBy.ExecutionCount, topCount);
                    data.MostExecutedStatements = Converter.ConvertToMostExecutedStatements(topExecuted);
                    // slowest
                    var topSlowest = statementStatsRepository.GetSummaryTotalStatementStatistics(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, DAL.Contracts.SummaryNormalizedStatementStatisticsOrderBy.MaxDuration, topCount);
                    data.MostSlowestStatements = Converter.ConvertToMostSlowestStatements(topSlowest);
                    result.Data = data;
                    result.IsSuccess = data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("relation-total")]
        [HttpPost]
        public IActionResult GetTotalRelationStatistics(GetTotalRelationStatisticsRequest request)
        {
            GetTotalRelationStatisticsResponse result = new GetTotalRelationStatisticsResponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var totalRelationStatsRepository = DALRepositories.GetTotalRelationStatisticsRepository();
                    result.Data = totalRelationStatsRepository.GetAllForRelation(request.RelationID, request.Filter.DateFrom, request.Filter.DateTo).ToList();
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
    }
}
