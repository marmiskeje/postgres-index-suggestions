using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI.Controllers
{
    public abstract class BaseApiController : Controller
    {
        private static readonly ContractConverter converter = new ContractConverter();
        protected ContractConverter Converter
        {
            get { return converter; }
        }
        protected DAL.Contracts.IRepositoriesFactory DALRepositories
        {
            get { return DAL.RepositoriesFactory.Instance; }
        }
        protected DBMS.Contracts.IRepositoriesFactory DBMSRepositories
        {
            get { return DBMS.Postgres.RepositoriesFactory.Instance; }
        }
        protected void HandleException(Action action, Action<Exception> onExceptionAction)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                onExceptionAction(ex);
            }
        }
    }
}
