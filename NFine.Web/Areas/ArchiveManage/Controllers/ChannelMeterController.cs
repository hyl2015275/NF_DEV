using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Web.Areas.ArchiveManage.Controllers
{
    public class ChannelMeterController : ControllerBase
    {
        private readonly ChannelMeterApp _channelMeterApp = new ChannelMeterApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _channelMeterApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _channelMeterApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ChannelMeterEntity channelMeterEntity, string keyValue)
        {
            _channelMeterApp.SubmitForm(channelMeterEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            _channelMeterApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public virtual ActionResult UpLoad()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var file = Request.Files[0];
            if (file != null) _channelMeterApp.UpLoad(file.InputStream, companyId);
            return Success("操作成功。");
        }
    }
}
