using NFine.Code;
using System.Web.Mvc;
using NFine.Application;
using NFine.Application.SystemSecurity;
using NFine.Domain.Entity.SystemSecurity;


namespace NFine.Web
{
    public class HandlerLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action执行后
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = ((ContentResult)filterContext.Result).ToJson();
            new LogApp().WriteDbLog(new LogEntity
            {
                F_ModuleName = "用户管理",
                F_Type = DbLogType.Delete.ToString(),
                F_Account = OperatorProvider.Provider.GetCurrent().UserCode,
                F_NickName = OperatorProvider.Provider.GetCurrent().UserName,
                F_Result = true,
                F_Description = filterContext.Result.ToString(),
            });
        }
    }
}