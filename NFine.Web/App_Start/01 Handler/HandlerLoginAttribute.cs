using NFine.Code;
using System.Web.Mvc;

namespace NFine.Web
{
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        public bool Ignore;
        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
            {
                return;
            }
            if (OperatorProvider.Provider.GetCurrent() != null) return;
            WebHelper.WriteCookie("nfine_login_error", "overdue");
            filterContext.HttpContext.Response.Write("<script>top.location.href = '/Login/Index';</script>");
        }
    }
}