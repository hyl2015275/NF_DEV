using System.Web.Mvc;

namespace NFine.Web.Areas.DataStatistics
{
    public class DataStatisticsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DataStatistics";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DataStatistics_default",
                "DataStatistics/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
