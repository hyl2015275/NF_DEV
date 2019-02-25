using System.Web.Mvc;

namespace NFine.Web.Areas.RevenueInterface
{
    public class RevenueInterfaceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RevenueInterface";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RevenueInterface_default",
                "RevenueInterface/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
