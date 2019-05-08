using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI.Controllers
{
    [Route("api/entities")]
    [ApiController]
    public class EntitiesController : BaseApiController
    {
        [Route("databases")]
        [HttpGet]
        public IActionResult GetDatabases()
        {
            BaseResponse<List<DatabaseData>> result = new BaseResponse<List<DatabaseData>>();
            HandleException(() =>
            {
                var repository = DBMSRepositories.GetDatabasesRepository();
                var databases = repository.GetAll();
                result.Data = new List<DatabaseData>();
                foreach (var d in databases)
                {
                    result.Data.Add(Converter.Convert(d));
                }
                result.IsSuccess = result.Data != null;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }

        [Route("relations")]
        [HttpGet]
        public IActionResult GetRelations()
        {
            BaseResponse<Dictionary<uint, List<RelationData>>> result = new BaseResponse<Dictionary<uint, List<RelationData>>>();
            HandleException(() =>
            {
                var databases = DBMSRepositories.GetDatabasesRepository().GetAll();
                var relationsRepository = DBMSRepositories.GetRelationsRepository();
                result.Data = new Dictionary<uint, List<RelationData>>();
                foreach (var d in databases)
                {
                    try
                    {
                        using (var scope = Converter.CreateDatabaseScope(d.ID))
                        {
                            var relations = relationsRepository.GetAllNonSystems().OrderBy(x => x.Name);
                            result.Data.Add(d.ID, new List<RelationData>());
                            foreach (var r in relations)
                            {
                                result.Data[d.ID].Add(Converter.Convert(r));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                result.IsSuccess = result.Data != null;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }

        [Route("indices")]
        [HttpGet]
        public IActionResult GetIndices()
        {
            BaseResponse<Dictionary<uint, List<IndexData>>> result = new BaseResponse<Dictionary<uint, List<IndexData>>>();
            HandleException(() =>
            {
                var databases = DBMSRepositories.GetDatabasesRepository().GetAll();
                var indicesRepository = DBMSRepositories.GetIndicesRepository();
                result.Data = new Dictionary<uint, List<IndexData>>();
                foreach (var d in databases)
                {
                    try
                    {
                        using (var scope = Converter.CreateDatabaseScope(d.ID))
                        {
                            var indices = indicesRepository.GetAllNonSystems().OrderBy(x => x.Name);
                            foreach (var i in indices)
                            {
                                if (!result.Data.ContainsKey(i.RelationID))
                                {
                                    result.Data.Add(i.RelationID, new List<IndexData>());
                                }
                                result.Data[i.RelationID].Add(Converter.Convert(i));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                result.IsSuccess = result.Data != null;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }

        [Route("stored-procedures")]
        [HttpGet]
        public IActionResult GetStoredProcedures()
        {
            BaseResponse<Dictionary<uint, List<StoredProcedureData>>> result = new BaseResponse<Dictionary<uint, List<StoredProcedureData>>>();
            HandleException(() =>
            {
                var databases = DBMSRepositories.GetDatabasesRepository().GetAll();
                var proceduresRepository = DBMSRepositories.GetStoredProceduresRepository();
                result.Data = new Dictionary<uint, List<StoredProcedureData>>();
                foreach (var d in databases)
                {
                    try
                    {
                        using (var scope = Converter.CreateDatabaseScope(d.ID))
                        {
                            var procedures = proceduresRepository.GetAllNonSystems().OrderBy(x => x.Name);
                            result.Data.Add(d.ID, new List<StoredProcedureData>());
                            foreach (var p in procedures)
                            {
                                result.Data[d.ID].Add(Converter.Convert(p));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
                result.IsSuccess = result.Data != null;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }
    }
}
