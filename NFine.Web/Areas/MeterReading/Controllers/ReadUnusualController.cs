using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.MeterReading;
using NFine.Code;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class ReadUnusualController : ControllerBase
    {
        private readonly ReadUnusualApp _readUnusualApp = new ReadUnusualApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _readUnusualApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _readUnusualApp.DownLoad(queryJson, companyId);
        }
        public ActionResult DeleteForm(string keyValue)
        {
            _readUnusualApp.DeleteForm(keyValue);
            return Success("手工转正常操作成功！");
        }
    }
}