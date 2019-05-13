
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : BaseApiController
    {
        [Route("configuration")]
        [HttpGet]
        public IActionResult GetConfiguration()
        {
            BaseResponse<ConfigurationData> result = new BaseResponse<ConfigurationData>();
            HandleException(() =>
            {
                var repository = DALRepositories.GetSettingPropertiesRepository();
                var data = new ConfigurationData();
                var smtp = repository.GetObject<DAL.Contracts.SmtpConfiguration>(DAL.Contracts.SettingPropertyKeys.SMTP_CONFIGURATION);
                data.Smtp = Converter.Convert(smtp);
                var reporting = repository.GetObject<DAL.Contracts.ReportingSettings>(DAL.Contracts.SettingPropertyKeys.REPORTING_SETTINGS);
                data.Reports = Converter.Convert(reporting);
                var collectorConfiguration = repository.GetObject<DAL.Contracts.CollectorConfiguration>(DAL.Contracts.SettingPropertyKeys.COLLECTOR_CONFIGURATION);
                data.Collector = Converter.Convert(collectorConfiguration);
                result.Data = data;
                result.IsSuccess = result.Data != null;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }

        [Route("update-configuration")]
        [HttpPost]
        public IActionResult UpdateConfiguration(ConfigurationData configuration)
        {
            BaseOperationResponse result = new BaseOperationResponse();
            HandleException(() =>
            {
                var repository = DALRepositories.GetSettingPropertiesRepository();
                var originalSmtp = repository.GetObject<DAL.Contracts.SmtpConfiguration>(DAL.Contracts.SettingPropertyKeys.SMTP_CONFIGURATION);
                var smtp = Converter.CreateSmtpConfiguration(configuration.Smtp, originalSmtp);
                repository.SetObject(DAL.Contracts.SettingPropertyKeys.SMTP_CONFIGURATION, smtp);
                var reporting = Converter.CreateReportingSettings(configuration.Reports);
                repository.SetObject(DAL.Contracts.SettingPropertyKeys.REPORTING_SETTINGS, reporting);
                var collector = Converter.CreateCollectorConfiguration(configuration.Collector);
                repository.SetObject(DAL.Contracts.SettingPropertyKeys.COLLECTOR_CONFIGURATION, collector);
                result.IsSuccess = true;
            }, ex => result.ErrorMessage = ex.Message);
            return Json(result);
        }
    }
}
