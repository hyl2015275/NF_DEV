using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.PaymentManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Enum;
using Spire.Xls;

namespace NFine.Web.Areas.PaymentManage.Controllers
{
    public class RefundOrderController : ControllerBase
    {
        private readonly RefundOrderApp _refundOrderApp = new RefundOrderApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly MeterChargingApp _meterChargingApp = new MeterChargingApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _refundOrderApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                totalSum = _refundOrderApp.GetSum(queryJson, companyId),
            };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(string F_UserCard, string F_Money)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var meter = _meterApp.GetFormByUserCard(F_UserCard, companyId);
            if (meter == null)
                throw new Exception("用户卡不存在");
            var meterCharging = _meterChargingApp.GetForm(meter.F_Id);
            meterCharging.F_Balance -= decimal.Parse(F_Money);
            _refundOrderApp.SubmitForm(new RefundOrderEntity
            {
                F_CreatorUserName = OperatorProvider.Provider.GetCurrent().UserName,
                F_Balance = meterCharging.F_Balance,
                F_ArchiveId = meter.F_Id,
                F_OrderNumber = Common.CreateNo(),
                F_Money = decimal.Parse(F_Money),
                F_OwnerId = companyId,
                F_RefundTime = DateTime.Now,
            }, meterCharging);
            return Success("操作成功。");
        }
        [HttpGet]
        public ActionResult SjzReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BaseReport(string keyValue)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var payOrderEntity = _refundOrderApp.GetForm(keyValue);
            var meterEntity = _meterApp.GetForm(payOrderEntity.F_ArchiveId);
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string template = tmpRootDir + "/Areas/PaymentManage/Template/通用收费模板.xls".Replace(@"/", @"\"); //转换成绝对路径
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(template);
            //将第一张工作表保存为图片
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Rows[0].Cells[0].Text = new OrganizeApp().GetForm(OperatorProvider.Provider.GetCurrent().CompanyId).F_FullName;
            sheet.Rows[1].Cells[1].Text = "7544";
            sheet.Rows[1].Cells[5].Text = payOrderEntity.F_RefundTime.Value.Year.ToString() + "年";
            sheet.Rows[1].Cells[6].Text = payOrderEntity.F_RefundTime.Value.Month.ToString() + "月";
            sheet.Rows[1].Cells[7].Text = payOrderEntity.F_RefundTime.Value.Day.ToString() + "日";
            sheet.Rows[1].Cells[9].Text = payOrderEntity.F_OrderNumber ?? "";
            sheet.Rows[2].Cells[1].Text = meterEntity.F_UserCard ?? "";
            sheet.Rows[2].Cells[6].Text = meterEntity.F_CustomerName ?? "";
            sheet.Rows[3].Cells[1].Text = meterEntity.F_MeterCode ?? "";
            sheet.Rows[3].Cells[6].Text = meterEntity.F_CustomerAddress ?? "";
            sheet.Rows[5].Cells[1].Text = ((Dictionary<string, string>)dataItems["DeviceType"])[meterEntity.F_MeterType]; ;
            sheet.Rows[5].Cells[5].Text = payOrderEntity.F_UnitPrice ?? "";
            sheet.Rows[5].Cells[7].Text = (payOrderEntity.F_PayTotal * -1).ToString("0.00");
            sheet.Rows[5].Cells[9].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money * -1)).ToString("0.00");
            sheet.Rows[8].Cells[1].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money * -1)).ToString("0.00");
            sheet.Rows[8].Cells[5].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money * -1)).ToString("0.00");
            sheet.Rows[8].Cells[8].Text = payOrderEntity.F_Balance == null ? "0" : ((decimal)(payOrderEntity.F_Balance)).ToString("0.00");
            sheet.Rows[9].Cells[1].Text = StringHelper.ConvertToChinese(Convert.ToDouble(payOrderEntity.F_Money ?? 0) * -1);
            sheet.Rows[10].Cells[9].Text = payOrderEntity.F_CreatorUserName ?? "admin";
            string imageUrl = tmpRootDir + "/Areas/PaymentManage/Template/Report/" + keyValue + ".jpg".Replace(@"/", @"\");
            string imageUrl2 = tmpRootDir + "/Areas/PaymentManage/Template/Report2/" + keyValue + ".jpg".Replace(@"/", @"\");
            sheet.SaveToImage(imageUrl);
            ImageHelper.CaptureImage(imageUrl, imageUrl2, 723, 320, 53, 73);
            return View();
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _refundOrderApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="queryJson">查询条件</param>
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            _refundOrderApp.DownLoad(queryJson, companyId);
        }
    }
}