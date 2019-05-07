using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class BaseOperationResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BaseResponse<TData> : BaseOperationResponse
    {
        public TData Data { get; set; }
    }
}
