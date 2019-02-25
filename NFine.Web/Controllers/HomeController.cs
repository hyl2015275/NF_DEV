/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.MeterReading;
using NFine.Application.PaymentManage;
using NFine.Domain.Enum;
using NFine.Application.DataStatistics;
using System.Threading;

namespace NFine.Web.Controllers
{
    [HandlerLogin]
    public class HomeController : Controller
    {
        private readonly ReportStatisticsApp _readRecordApp = new ReportStatisticsApp();
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        private readonly ChargeRecordApp _chargeRecordApp = new ChargeRecordApp();
        private readonly ReadUnusualApp _readUnusualApp = new ReadUnusualApp();
        private readonly MeterApp _meterApp = new MeterApp();
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult Default()
        {
            if (OperatorProvider.Provider.GetCurrent().IsPlatform)
                return Redirect("Platform");
            return View();
        }

        [HttpGet]
        public ActionResult Platform()
        {
            return View();
        }

        public ActionResult GetTotalData()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var meterCount = _meterApp.GetCountTotal(companyId);
            var unusualCount = _readUnusualApp.GetCountTotal(companyId);
            var recordSum = _chargeRecordApp.GetSumTotal(companyId);
            var paySum = _payOrderApp.GetSumToal(companyId);
            var data = new[] { meterCount.ToString(), (meterCount - unusualCount).ToString(), unusualCount.ToString(), recordSum.ToString("#0.00"), paySum.ToString("#0.00") };
            return Content(data.ToJson());
        }

        public ActionResult GetLeaveChart()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var wechatPay = ((int)PayTypeEnum.WeChatPay).ToString();
            var aliPay = ((int)PayTypeEnum.AliPay).ToString();
            var businessPay = ((int)PayTypeEnum.BusinessPay).ToString();


            string a = "", b = "", c = "";

            a = _payOrderApp.GetCountTotalByPayType(wechatPay, companyId).ToString();
            b = _payOrderApp.GetCountTotalByPayType(aliPay, companyId).ToString();
            c = _payOrderApp.GetCountTotalByPayType(businessPay, companyId).ToString();


            var data = new[]
            {
               a,b,c
            };
            return Content(data.ToJson());
        }

        public ActionResult GetSalaryChart()
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var lasels = new string[12];
            var now = System.DateTime.Now;
            for (var i = 1; i <= 12; i++)
            {
                lasels[12 - i] = now.AddMonths(-i).Month + "月";
            }
            var data = new
            {
                labels = lasels,
                data = _readRecordApp.GetMonthTotalByMeterType(new string[]{
                MeterTypeEnum.WaterMeter.ToString(),
                MeterTypeEnum.WattMeter.ToString(),
                MeterTypeEnum.GasMeter.ToString(),
                MeterTypeEnum.HeatMeter.ToString(),
                }, companyId),
            };
            return Content(data.ToJson());
        }
    }
}