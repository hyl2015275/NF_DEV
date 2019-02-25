using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace NFine.Domain.MBus.Models
{
    public class ApiResultModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}