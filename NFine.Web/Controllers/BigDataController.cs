using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.DataStatistics;
using NFine.Application.MeterReading;
using NFine.Application.PaymentManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Enum;
using NFine.Web.Models;

namespace NFine.Web.Controllers
{
    [HandlerLogin]
    public class BigDataController : Controller
    {
        private readonly ReportStatisticsApp _readRecordApp = new ReportStatisticsApp();
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        private readonly ChargeRecordApp _chargeRecordApp = new ChargeRecordApp();
        private readonly ReadUnusualApp _readUnusualApp = new ReadUnusualApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly OrganizeApp _organizeApp = new OrganizeApp();
        private readonly ReportStatisticsApp _reportStatisticsApp = new ReportStatisticsApp();
        private readonly StatisticsApp _statisticsApp = new StatisticsApp();
        public ActionResult Index()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var compay = _organizeApp.GetForm(companyId);
            ViewData["compayName"] = compay != null && !string.IsNullOrEmpty(compay.F_FullName) ? compay.F_FullName : "企业";
            return View();
        }
        public ActionResult GetTotalData()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var recordSum = _chargeRecordApp.GetSumTotal(companyId);
            var paySum = _payOrderApp.GetSumToal(companyId);
            var meterCount = _meterApp.GetCountTotal(companyId);
            var unusualCount = _readUnusualApp.GetCountTotal(companyId);
            var normalCount = meterCount - unusualCount;
            var uploadRate = meterCount == 0 ? 0 : ((normalCount * 100) / meterCount);
            var todaySales = _chargeRecordApp.GetSumTotal(companyId, DateTime.Now.Date, DateTime.Now.AddDays(1).Date);
            var yesterdaySales = _chargeRecordApp.GetSumTotal(companyId, DateTime.Now.AddDays(-1).Date, DateTime.Now.Date);
            var salesRise = yesterdaySales == 0 ? 0 : ((todaySales - yesterdaySales) / yesterdaySales) * 100;
            var data = new[] { meterCount.ToString(), normalCount.ToString(), uploadRate.ToString("#0") + "%", todaySales.ToString("#0.00"), yesterdaySales.ToString("#0.00"), salesRise.ToString("#0") + "%", recordSum.ToString("#0.00"), paySum.ToString("#0.00"), };
            return Content(data.ToJson());
        }
        public ActionResult GetPrestoreChart()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var meterList = _meterApp.GetAllList(companyId);
            var v0 = meterList.Average(x => x.F_Balance);
            var v1 = meterList.Count(x => x.F_Balance >= 0);
            var v2 = meterList.Count(x => x.F_Balance < 0);
            var data = new[] { v0, v1, v2 };
            return Content(data.ToJson());
        }
        public ActionResult GetPayTypeTotal()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var payOrderList = _payOrderApp.GetList(DateTime.MinValue, DateTime.Now, companyId);
            var data = new[] { payOrderList.Count(x => x.F_PayType == ((int)PayTypeEnum.BusinessPay).ToString()), payOrderList.Count(x => x.F_PayType == ((int)PayTypeEnum.AliPay).ToString()), payOrderList.Count(x => x.F_PayType == ((int)PayTypeEnum.WeChatPay).ToString()) };
            return Content(data.ToJson());
        }

        public ActionResult GetMonthlyReport()
        {
            var i = 1;
            var data = new MonthlyReportViewModel();
            var record = new List<MonthlyReportViewModel.Record>();
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var reportCyde = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
            var reportStatistics = _reportStatisticsApp.GetFormByReportCyde(reportCyde, companyId);
            data.data = new[] { reportStatistics.Sum(x => x.F_SumDosage), reportStatistics.Sum(x => x.F_SumBill), reportStatistics.Sum(x => x.F_SumPay) };
            var costList = _statisticsApp.GetCustomerStatisticsList(reportCyde, companyId).GroupBy(x => x.F_CustomerName);
            foreach (var item in costList)
            {
                record.Add(new MonthlyReportViewModel.Record
                {
                    serial = i++,
                    name = item.Key,
                    sumcost = item.Sum(x => x.F_ThisMonthBill)
                });
            }
            data.record = record.ToArray();
            return Content(data.ToJson());
        }
        public ActionResult GetPayOrderReport()
        {
            var i = 1;
            var record = new List<PayOrderReportViewModel>();
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var payList = _payOrderApp.GetList(new Pagination()
            {
                page = 1,
                rows = 10,
                sidx = "F_PaymentTime",
                sord = "desc"
            }, "{}", companyId);
            foreach (var item in payList)
            {
                record.Add(new PayOrderReportViewModel
                {
                    serial = i++,
                    name = item.F_CustomerName,
                    money = item.F_Money ?? 0,
                    type = item.F_PayType
                });
            }
            return Content(record.ToArray().ToJson());
        }
    }
}