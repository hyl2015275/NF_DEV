using System.Web.Mvc;

namespace NFine.Web.Areas.PaymentManage
{
    public class PaymentManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PaymentManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PaymentManage_default",
                "PaymentManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
