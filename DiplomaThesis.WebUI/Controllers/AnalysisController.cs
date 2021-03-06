﻿
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI.Controllers
{
    [Route("api/analysis")]
    [ApiController]
    public class AnalysisController : BaseApiController
    {
        [Route("workload-analysis-detail")]
        [HttpPost]
        public IActionResult GetWorkloadAnalysisDetail(GetWorkloadAnalysisDetailRequest request)
        {
            GetWorkloadAnalysisDetailReponse result = new GetWorkloadAnalysisDetailReponse();
            if (request != null)
            {
                HandleException(() =>
                {
                    var environmentsRepository = DALRepositories.GetVirtualEnvironmentsRepository();
                    var indicesRepository = DALRepositories.GetPossibleIndicesRepository();

                    var indicesEnvs = environmentsRepository.GetAllForWorkloadAnalysis(request.WorkloadAnalysisID, DAL.Contracts.VirtualEnvironmentType.Indices);
                    var hPartsEnvs = environmentsRepository.GetAllForWorkloadAnalysis(request.WorkloadAnalysisID, DAL.Contracts.VirtualEnvironmentType.HPartitionings);
                    var indicesIds = indicesEnvs.SelectMany(x => x.VirtualEnvironmentPossibleIndices).Select(x => x.PossibleIndexID).ToHashSet();
                    var indices = indicesRepository.GetByIds(indicesIds);

                    result.Data = Converter.ConvertToWorkloadAnalysisDetail(indicesEnvs, hPartsEnvs, indices);
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }

        [Route("workload-analysis-detail-env")]
        [HttpPost]
        public IActionResult GetWorkloadAnalysisDetailForEnvironment(GetWorkloadAnalysisDetailForEnvRequest request)
        {
            GetWorkloadAnalysisDetailForEnvReponse result = new GetWorkloadAnalysisDetailForEnvReponse();
            if (request != null)
            {
                HandleException(() =>
                {
                    var environmentsRepository = DALRepositories.GetVirtualEnvironmentsRepository();
                    var realStatementEvaluationsRepository = DALRepositories.GetWorkloadAnalysisRealStatementEvaluationsRepository();

                    var env = environmentsRepository.GetDetail(request.EnvironmentID);
                    var statements = realStatementEvaluationsRepository.GetAllForWorkloadAnalysis(request.WorkloadAnalysisID);

                    result.Data = Converter.ConvertToWorkloadAnalysisDetailForEnv(env, statements);
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("workload-analysis-detail-env-best")]
        [HttpPost]
        public IActionResult GetWorkloadAnalysisDetailForBestEnvironment(GetWorkloadAnalysisDetailForBestEnvRequest request)
        {
            GetWorkloadAnalysisDetailForEnvReponse result = new GetWorkloadAnalysisDetailForEnvReponse();
            if (request != null)
            {
                HandleException(() =>
                {
                    var environmentsRepository = DALRepositories.GetVirtualEnvironmentsRepository();
                    var realStatementEvaluationsRepository = DALRepositories.GetWorkloadAnalysisRealStatementEvaluationsRepository();
                    var environmentID = environmentsRepository.GetBestEnvironmentForWorkloadAnalysis(request.WorkloadAnalysisID);
                    if (environmentID > 0)
                    {
                        var env = environmentsRepository.GetDetail(environmentID);
                        var statements = realStatementEvaluationsRepository.GetAllForWorkloadAnalysis(request.WorkloadAnalysisID);

                        result.Data = Converter.ConvertToWorkloadAnalysisDetailForEnv(env, statements); 
                    }
                    else
                    {
                        result.Data = new WorkloadAnalysisDetailForEnvData();
                    }
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }

        [Route("workload-analysis-create")]
        [HttpPost]
        public IActionResult CreateWorkloadAnalysis(CreateWorkloadAnalysisRequest request)
        {
            BaseOperationResponse result = new BaseOperationResponse() { IsSuccess = false };
            (bool isValid, string errorMessage) = WorkloadAnalysisValidator.Validate(request);
            if (!isValid)
            {
                result.ErrorMessage = errorMessage;
            }
            else
            {
                HandleException(() =>
                {
                    var workloadAnalysis = Converter.CreateWorkloadAnalysis(request);
                    var repository = DALRepositories.GetWorkloadAnalysesRepository();
                    repository.Create(workloadAnalysis);
                    result.IsSuccess = true;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
        [Route("workload-analyses")]
        [HttpPost]
        public IActionResult GetWorkloadAnalyses(GetWorkloadAnalysesRequest request)
        {
            GetWorkloadAnalysesReponse result = new GetWorkloadAnalysesReponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var repository = DALRepositories.GetWorkloadAnalysesRepository();
                    var analyses = repository.GetForDatabase(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo);
                    result.Data = new List<WorkloadAnalysisData>();
                    foreach (var a in analyses)
                    {
                        result.Data.Add(Converter.Convert(a));
                    }
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }

        [Route("workloads")]
        [HttpPost]
        public IActionResult GetWorkloads(GetWorkloadsRequest request)
        {
            GetWorkloadsReponse result = new GetWorkloadsReponse();
            if (request != null && request.Filter != null)
            {
                HandleException(() =>
                {
                    var repository = DALRepositories.GetWorkloadsRepository();
                    var workloads = repository.GetForDatabase(request.DatabaseID, request.Filter.DateFrom, request.Filter.DateTo, onlyActive: true);
                    result.Data = new List<WorkloadData>();
                    foreach (var w in workloads)
                    {
                        result.Data.Add(Converter.Convert(w));
                    }
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }

        [Route("workload-create")]
        [HttpPost]
        public IActionResult CreateWrokload(WorkloadData workloadData)
        {
            BaseOperationResponse result = new BaseOperationResponse() { IsSuccess = false };
            (bool isValid, string errorMessage) = WorkloadValidator.Validate(workloadData);
            if (!isValid)
            {
                result.ErrorMessage = errorMessage;
            }
            else
            {
                HandleException(() =>
                {
                    var workload = Converter.CreateWorkload(workloadData);
                    var repository = DALRepositories.GetWorkloadsRepository();
                    repository.Create(workload);
                    result.IsSuccess = true;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }

        [Route("workload-delete")]
        [HttpPut]
        public IActionResult DeleteWorkload(DeleteWorkloadRequest request)
        {
            BaseOperationResponse result = new BaseOperationResponse() { IsSuccess = false };
            if (request != null)
            {
                HandleException(() =>
                {
                    var repository = DALRepositories.GetWorkloadsRepository();
                    var workload = repository.GetByPrimaryKey(request.WorkloadID);
                    if (workload != null)
                    {
                        if (workload.InactiveDate == null)
                        {
                            workload.InactiveDate = DateTime.Now;
                            repository.Update(workload);
                        }
                        result.IsSuccess = true;
                    }
                }, ex => result.ErrorMessage = ex.Message); 
            }
            return Json(result);
        }

        [Route("unused-objects")]
        [HttpPost]
        public IActionResult GetUnusedObjects(GetUnusedObjectsRequest request)
        {
            GetUnusedObjectsResponse result = new GetUnusedObjectsResponse();
            if (request != null)
            {
                HandleException(() =>
                {
                    result.Data = new UnusedObjectsData();
                    result.Data.Objects = new List<UnusedObjectData>();
                    var now = DateTime.Now;
                    DateTime thresholdDate = now.AddDays(-30);
                    var totalIndexStatsRepo = DALRepositories.GetTotalIndexStatisticsRepository();
                    var totalRelationStatsRepo = DALRepositories.GetTotalRelationStatisticsRepository();
                    var totalProceduresStatsRepo = DALRepositories.GetTotalStoredProcedureStatisticsRepository();
                    var totalViewsStatsRepo = DALRepositories.GetTotalViewStatisticsRepository();
                    bool areDataAvailableForWholePeriod = totalIndexStatsRepo.AreDataAvailableForWholePeriod(thresholdDate, now.Date)
                                                       || totalRelationStatsRepo.AreDataAvailableForWholePeriod(thresholdDate, now.Date)
                                                       || totalProceduresStatsRepo.AreDataAvailableForWholePeriod(thresholdDate, now.Date)
                                                       || totalViewsStatsRepo.AreDataAvailableForWholePeriod(thresholdDate, now.Date);
                    var indicesLastKnownCollectionDate = totalIndexStatsRepo.GetForAllLastKnownCollectionDate(request.DatabaseID);
                    var relationsLastKnownCollectionDate = totalRelationStatsRepo.GetForAllLastKnownCollectionDate(request.DatabaseID);
                    var proceduresLastKnownCollectionDate = totalProceduresStatsRepo.GetForAllLastKnownCollectionDate(request.DatabaseID);
                    var viewsLastKnownCollectionDate = totalViewsStatsRepo.GetForAllLastKnownCollectionDate(request.DatabaseID);
                    using (var scope = Converter.CreateDatabaseScope(request.DatabaseID))
                    {
                        foreach (var i in DBMSRepositories.GetIndicesRepository().GetAll())
                        {
                            bool? isUnused = null;
                            DateTime? lastKnownUsageDate = null;
                            if (indicesLastKnownCollectionDate.TryGetValue(i.ID, out var lastKnownCollectionDate))
                            {
                                lastKnownUsageDate = lastKnownCollectionDate;
                                if (areDataAvailableForWholePeriod)
                                {
                                    isUnused = lastKnownCollectionDate < thresholdDate;
                                }
                            }
                            result.Data.Objects.Add(new UnusedObjectData()
                            {
                                IsUnused = isUnused, LastKnownUsageDate = lastKnownUsageDate,
                                Name = i.Name + $" ON {i.SchemaName}.{i.RelationName}", ObjectType = "Index", SchemaName = i.SchemaName
                            });
                        }
                        foreach (var r in DBMSRepositories.GetRelationsRepository().GetAll())
                        {
                            bool? isUnused = null;
                            DateTime? lastKnownUsageDate = null;
                            if (relationsLastKnownCollectionDate.TryGetValue(r.ID, out var lastKnownCollectionDate))
                            {
                                lastKnownUsageDate = lastKnownCollectionDate;
                                if (areDataAvailableForWholePeriod)
                                {
                                    isUnused = lastKnownCollectionDate < thresholdDate;
                                }
                            }
                            result.Data.Objects.Add(new UnusedObjectData()
                            {
                                IsUnused = isUnused,
                                LastKnownUsageDate = lastKnownUsageDate,
                                Name = r.Name,
                                ObjectType = "Relation",
                                SchemaName = r.SchemaName
                            });
                        }
                        foreach (var p in DBMSRepositories.GetStoredProceduresRepository().GetAll())
                        {
                            bool? isUnused = null;
                            DateTime? lastKnownUsageDate = null;
                            if (proceduresLastKnownCollectionDate.TryGetValue(p.ID, out var lastKnownCollectionDate))
                            {
                                lastKnownUsageDate = lastKnownCollectionDate;
                                if (areDataAvailableForWholePeriod)
                                {
                                    isUnused = lastKnownCollectionDate < thresholdDate;
                                }
                            }
                            result.Data.Objects.Add(new UnusedObjectData()
                            {
                                IsUnused = isUnused,
                                LastKnownUsageDate = lastKnownUsageDate,
                                Name = p.Name,
                                ObjectType = "Stored procedure",
                                SchemaName = p.SchemaName
                            });
                        }
                        foreach (var v in DBMSRepositories.GetViewsRepository().GetAll())
                        {
                            bool? isUnused = null;
                            DateTime? lastKnownUsageDate = null;
                            if (viewsLastKnownCollectionDate.TryGetValue(v.ID, out var lastKnownCollectionDate))
                            {
                                lastKnownUsageDate = lastKnownCollectionDate;
                                if (areDataAvailableForWholePeriod)
                                {
                                    isUnused = lastKnownCollectionDate < thresholdDate;
                                }
                            }
                            result.Data.Objects.Add(new UnusedObjectData()
                            {
                                IsUnused = isUnused,
                                LastKnownUsageDate = lastKnownUsageDate,
                                Name = v.Name,
                                ObjectType = "View",
                                SchemaName = v.SchemaName
                            });
                        }
                    }
                    result.Data.Objects.Sort((x, y) =>
                    {
                        var cmp = (x.LastKnownUsageDate?.Ticks ?? double.MaxValue).CompareTo(y.LastKnownUsageDate?.Ticks ?? double.MaxValue);
                        if (cmp == 0)
                        {
                            cmp = x.SchemaName.CompareTo(y.SchemaName);
                            if (cmp == 0)
                            {
                                cmp = x.Name.CompareTo(y.Name);
                            }
                        }
                        return cmp;
                    });
                    result.IsSuccess = result.Data != null;
                }, ex => result.ErrorMessage = ex.Message);
            }
            return Json(result);
        }
    }
}
