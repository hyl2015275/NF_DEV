using System.Web.Mvc;

namespace NFine.Web.Areas.ArchiveManage
{
    public class ArchiveManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ArchiveManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ArchiveManage_default",
                "ArchiveManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}