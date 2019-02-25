using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.PaymentManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.ViewModel.PaymentManage;

namespace NFine.Web.Areas.DataStatistics.Controllers
{
    public class RechargeReportController : ControllerBase
    {
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetReport(DateTime reportTime, string type = "day")
        {
            List<RechargeReportViewModel> data;
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            switch (type)
            {
                case "day":
                    data = _payOrderApp.GetDailyData(reportTime, companyId);
                    break;
                case "month":
                    data = _payOrderApp.GetMonthlyReport(reportTime, companyId); ;
                    break;
                case "year":
                    data = _payOrderApp.GetAnnualReport(reportTime, companyId); ;
                    break;
                default:
                    data = new List<RechargeReportViewModel>();
                    break;
            }
            return Content(data.ToJson());
        }
        [HttpGet]
        public virtual void DownLoad(DateTime reportTime, string type = "day")
        {
            List<RechargeReportViewModel> data;
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            switch (type)
            {
                case "day":
                    data = _payOrderApp.GetDailyData(reportTime, companyId);
                    break;
                case "month":
                    data = _payOrderApp.GetMonthlyReport(reportTime, companyId); ;
                    break;
                case "year":
                    data = _payOrderApp.GetAnnualReport(reportTime, companyId); ;
                    break;
                default:
                    data = new List<RechargeReportViewModel>();
                    break;
            }
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("购买日期"),
                new DataColumn("购买人次"),
                new DataColumn("累计数量"),
                new DataColumn("累计金额"),
                new DataColumn("备注"),
            });
            foreach (var en in data)
            {
                var row = dt.NewRow();
                row[0] = en.F_PurchaseDate;
                row[1] = en.F_PurchaseNumber;
                row[2] = en.F_AccumulativeTotal.ToString("0.00");
                row[3] = en.F_AccumulativeAmount.ToString("0.00");
                row[4] = en.F_Description;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "缴费统计", "缴费统计.xls");
        }
    }
}