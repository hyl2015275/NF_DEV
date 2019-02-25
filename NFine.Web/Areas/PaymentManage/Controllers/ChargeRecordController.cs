using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.PaymentManage;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;

namespace NFine.Web.Areas.PaymentManage.Controllers
{
    public class ChargeRecordController : ControllerBase
    {
        private readonly ChargeRecordApp _chargeRecordApp = new ChargeRecordApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _chargeRecordApp.GetList(pagination, keyword, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                totalSum = _chargeRecordApp.GetSum(keyword, companyId),
            };
            return Content(data.ToJson());
        }

    }
}