using NFine.Code;
using System.Web.Mvc;

namespace NFine.Web
{
    public class HandlerErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!string.IsNullOrEmpty(context.Exception.Source) && context.Exception.Source == "timeout")
            {
                WebHelper.WriteCookie("nfine_login_error", "overdue");
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 200;
                context.HttpContext.Response.Write("<script>top.location.href = '/Login/Index';</script>");
            }
            else
            {
                base.OnException(context);
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 200;
                context.Result = new ContentResult { Content = new AjaxResult { state = ResultType.error.ToString(), message = context.Exception.Message}.ToJson() };
                WriteLog(context);
            }
        }
        private void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            var log = LogFactory.GetLogger(context.Controller.ToString());
            log.Error(context.Exception);
        }
    }
}