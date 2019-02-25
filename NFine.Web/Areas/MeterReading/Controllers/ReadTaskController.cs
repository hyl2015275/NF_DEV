using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class ReadTaskController : ControllerBase
    {
        private readonly ReadTaskApp _readTaskApp = new ReadTaskApp();
        private readonly MeterApp _meterApp = new MeterApp();
        [HttpGet]
        public ActionResult TaskList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult TaskForm()
        {
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _readTaskApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult DeleteForm(string keyValue)
        {
            _readTaskApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitForm(ReadTaskEntity roleEntity, string keyValue)
        {
            _readTaskApp.SubmitForm(roleEntity, keyValue);
            return Success("操作成功。");
        }
    }
}
