using System.Web.Mvc;

namespace NFine.Web.Areas.MeterMaintain
{
    public class MeterMaintainAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MeterMaintain";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MeterMaintain_default",
                "MeterMaintain/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
