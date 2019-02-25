using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using NFine.Domain.MBus.Models;

namespace NFine.Web
{
    public class ApiResultAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            base.OnActionExecuted(actionExecutedContext);

            var result = new ApiResultModel();
            if (actionExecutedContext.Exception != null)
            {
                result.ErrorMessage = actionExecutedContext.Exception.Message;
                //请求是否成功
                result.IsSuccess = false;
                //结果转为自定义消息格式
                HttpResponseMessage httpResponseMessage = JsonHelper.ToJson(result);
                // 重新封装回传格式
                actionExecutedContext.Response = httpResponseMessage;
            }
            else
            {
                // 取得由 API 返回的资料
                result.Data = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result;
                //请求是否成功
                result.IsSuccess = actionExecutedContext.ActionContext.Response.IsSuccessStatusCode;
                //结果转为自定义消息格式
                HttpResponseMessage httpResponseMessage = JsonHelper.ToJson(result);
                // 重新封装回传格式
                actionExecutedContext.Response = httpResponseMessage;
            }
        }
    }
}