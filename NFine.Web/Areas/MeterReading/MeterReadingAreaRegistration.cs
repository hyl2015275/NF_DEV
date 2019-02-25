using System.Web.Mvc;

namespace NFine.Web.Areas.MeterReading
{
    public class MeterReadingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MeterReading";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MeterReading_default",
                "MeterReading/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
