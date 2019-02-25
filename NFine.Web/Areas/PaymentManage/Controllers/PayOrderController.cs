using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using NFine.Application.ArchiveManage;
using NFine.Application.DataStatistics;
using NFine.Application.PaymentManage;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Enum;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using Spire.Xls;

namespace NFine.Web.Areas.PaymentManage.Controllers
{
    public class PayOrderController : ControllerBase
    {
        private readonly PayOrderApp _payOrderApp = new PayOrderApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly MeterChargingApp _meterChargingApp = new MeterChargingApp();
        private readonly MoneyAlarmApp _moneyAlarmApp = new MoneyAlarmApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _payOrderApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                totalSum = _payOrderApp.GetSum(queryJson, companyId),
            };
            return Content(data.ToJson());
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
            var payOrderEntity = _payOrderApp.GetForm(keyValue);
            var meterEntity = _meterApp.GetForm(payOrderEntity.F_ArchiveId);
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string template = tmpRootDir + "/Areas/PaymentManage/Template/通用收费模板.xls".Replace(@"/", @"\"); //转换成绝对路径
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(template);
            //将第一张工作表保存为图片
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Rows[0].Cells[0].Text = new OrganizeApp().GetForm(OperatorProvider.Provider.GetCurrent().CompanyId).F_FullName;
            sheet.Rows[1].Cells[1].Text = "7544";
            sheet.Rows[1].Cells[5].Text = payOrderEntity.F_PaymentTime.Value.Year.ToString() + "年";
            sheet.Rows[1].Cells[6].Text = payOrderEntity.F_PaymentTime.Value.Month.ToString() + "月";
            sheet.Rows[1].Cells[7].Text = payOrderEntity.F_PaymentTime.Value.Day.ToString() + "日";
            sheet.Rows[1].Cells[9].Text = payOrderEntity.F_OrderNumber ?? "";
            sheet.Rows[2].Cells[1].Text = meterEntity.F_UserCard ?? "";
            sheet.Rows[2].Cells[6].Text = meterEntity.F_CustomerName ?? "";
            sheet.Rows[3].Cells[1].Text = meterEntity.F_MeterCode ?? "";
            sheet.Rows[3].Cells[6].Text = meterEntity.F_CustomerAddress ?? "";
            sheet.Rows[5].Cells[1].Text = ((Dictionary<string, string>)dataItems["DeviceType"])[meterEntity.F_MeterType];
            sheet.Rows[5].Cells[5].Text = payOrderEntity.F_UnitPrice ?? "";
            sheet.Rows[5].Cells[7].Text = payOrderEntity.F_PayTotal.ToString("0.00");
            sheet.Rows[5].Cells[9].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money)).ToString("0.00");
            sheet.Rows[8].Cells[1].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money)).ToString("0.00");
            sheet.Rows[8].Cells[5].Text = payOrderEntity.F_Money == null ? "0" : ((decimal)(payOrderEntity.F_Money)).ToString("0.00");
            sheet.Rows[8].Cells[8].Text = payOrderEntity.F_Balance == null ? "0" : ((decimal)(payOrderEntity.F_Balance)).ToString("0.00");
            sheet.Rows[9].Cells[1].Text = StringHelper.ConvertToChinese(Convert.ToDouble(payOrderEntity.F_Money ?? 0));
            sheet.Rows[10].Cells[9].Text = payOrderEntity.F_CreatorUserName ?? "admin";
            string imageUrl = tmpRootDir + "/Areas/PaymentManage/Template/Report/" + keyValue + ".jpg".Replace(@"/", @"\");
            string imageUrl2 = tmpRootDir + "/Areas/PaymentManage/Template/Report2/" + keyValue + ".jpg".Replace(@"/", @"\");
            //sheet.SaveToImage(imageUrl,ImageFormat.Jpeg);
            string filpath = tmpRootDir + "/Areas/PaymentManage/Template/Report/" + keyValue + ".cshtml".Replace(@"/", @"\");
            sheet.SaveToHtml(filpath);
            //ImageHelper.CaptureImage(imageUrl, imageUrl2, 723, 320, 53, 73);
            return View();
        }

        [HttpGet]
        public ActionResult BillReport(DateTime begintime, DateTime endtime)
        {
            var keyValue = Common.GuId();
            int number = 3;
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var payList = _payOrderApp.GetList(begintime, endtime, companyId);
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string template = tmpRootDir + "/Areas/PaymentManage/Template/通用打印模板.xls".Replace(@"/", @"\"); //转换成绝对路径
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(template);
            //将第一张工作表保存为图片
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Rows[0].Cells[0].Text = new OrganizeApp().GetForm(OperatorProvider.Provider.GetCurrent().CompanyId).F_FullName + "销售记录";
            sheet.Rows[1].Cells[1].Text = begintime.ToString("yyyy-MM-dd");
            sheet.Rows[1].Cells[3].Text = endtime.ToString("yyyy-MM-dd");
            //Add style of specified range  
            CellStyle style = workbook.Styles.Add("Style");
            style.Font.Size = 10;
            if (payList.Count > 0)
            {
                sheet.InsertRow(4, payList.Count);
                foreach (var item in payList)
                {
                    sheet.Rows[number].Cells[0].Text = item.F_UserCard ?? "-";
                    sheet.Rows[number].Cells[1].Text = item.F_MeterCode ?? "-";
                    sheet.Rows[number].Cells[2].Text = item.F_CustomerName ?? "-";
                    sheet.Rows[number].Cells[3].Text = item.F_CustomerAddress ?? "-";
                    sheet.Rows[number].Cells[4].Text = ((Dictionary<string, string>)dataItems["DeviceType"])[item.F_MeterType];
                    sheet.Rows[number].Cells[5].Text = item.F_PayTotal.ToString("0.00");
                    sheet.Rows[number].Cells[6].Text = item.F_Money == null ? "0" : ((decimal)(item.F_Money)).ToString("0.00");
                    sheet.Rows[number].Cells[7].Text = item.F_PaymentTime.Value.ToString("yyy-MM-dd");
                    sheet.Rows[number].Cells[8].Text = item.F_CreatorUserName ?? "-";
                    for (int i = 0; i <= 8; i++)
                    {
                        sheet.Rows[number].Cells[i].RowHeight = 15;
                        sheet.Rows[number].Cells[i].Style = style;
                    }
                    number++;
                }
                CellRange range = sheet.Range["A4:I" + (3 + payList.Count).ToString()];
                range.BorderInside(LineStyleType.Thin, Color.Black);
                range.BorderAround(LineStyleType.Thin, Color.Black);
            }
            //string imageUrl = tmpRootDir + "/Areas/PaymentManage/Template/Report/" + keyValue + ".jpg".Replace(@"/", @"\");
            //string imageUrl2 = tmpRootDir + "/Areas/PaymentManage/Template/Report2/" + keyValue + ".jpg".Replace(@"/", @"\");
            //sheet.SaveToImage(imageUrl);

            string filpath = tmpRootDir + "/Areas/PaymentManage/Template/Report/" + keyValue + ".cshtml".Replace(@"/", @"\");
            sheet.SaveToHtml(filpath);

            // ImageHelper.CaptureImage(imageUrl, imageUrl2, 723, 90 + payList.Count * 22, 53, 73);
            TempData["keyValue"] = keyValue;
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _payOrderApp.GetForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPayOrderCount(string deviceId)
        {
            return Content(_payOrderApp.CheckState(deviceId) ? "-1" : (_payOrderApp.GetPayOrderCount(deviceId) % 15 + 1).ToString());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(string F_UserCard, string F_Money, string F_Recharge)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var meter = _meterApp.GetFormByUserCard(F_UserCard, companyId);
            if (meter == null) throw new Exception("用户卡不存在");
            var meterCharging = _meterChargingApp.GetForm(meter.F_Id);
            meterCharging.F_Balance += decimal.Parse(F_Money);
            var payOrder = new PayOrderEntity
            {
                F_CreatorUserName = OperatorProvider.Provider.GetCurrent().UserName,
                F_State = ((int)PayStateEnum.Wait).ToString(),
                F_Balance = meterCharging.F_Balance,
                F_ArchiveId = meter.F_Id,
                F_OrderNumber = Common.CreateNo(),
                F_OutOrder = "",
                F_Money = decimal.Parse(F_Money),
                F_OwnerId = companyId,
                F_PaymentTime = DateTime.Now,
                F_PayType = ((int)PayTypeEnum.BusinessPay).ToString()
            };
            ReadTaskEntity controlTask = null;
            if (meterCharging.F_MeterModel != ((int)MeterModelEnum.CardPrepay).ToString())//非IC卡预付费
            {
                payOrder.F_State = ((int)PayStateEnum.Finish).ToString();
                if (meter.F_MeterType == MeterTypeEnum.WattMeter.ToString() && meterCharging.F_EnableClose == true &&
                    meterCharging.F_Balance > meterCharging.F_CloseAmount) //电表合闸命令
                {
                    controlTask = new ReadTaskEntity
                    {
                        F_Id = Common.GuId(),
                        F_CreatorTime = DateTime.Now,
                        F_MeterCode = meter.F_MeterCode,
                        F_Factor = meter.F_Factor,
                        F_MeterType = meter.F_MeterType,
                        F_State = (int)TaskStateEnum.Wait,
                        F_TaskType = (int)TaskTypeEnum.CloseSwitch,
                    };
                }
            }
            else if (F_Recharge == ((int)RechargeEnum.Card).ToString())//IC卡预付费卡充值
            {
                payOrder.F_State = ((int)PayStateEnum.Write).ToString();
            }
            else//IC卡预付费互联网充值
            {
                payOrder.F_State = ((int)PayStateEnum.Wait).ToString();
                controlTask = new ReadTaskEntity
                {
                    F_Id = Common.GuId(),
                    F_CreatorTime = DateTime.Now,
                    F_MeterCode = meter.F_MeterCode,
                    F_Factor = meter.F_Factor,
                    F_MeterType = meter.F_MeterType,
                    F_Param = decimal.Parse(F_Money).ToString("0.00"),
                    F_State = (int)TaskStateEnum.Wait,
                    F_TaskType = (int)TaskTypeEnum.Pay,
                };
            }
            if (companyId == "bd602a97-eb06-4a5e-9c36-30eb45bc717b")//临朐清源充值确认
            {
                _moneyAlarmApp.SureAlarm(meter.F_Id);
            }
            _payOrderApp.SubmitForm(payOrder, meterCharging, controlTask);
            return Success("操作成功。");
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
            _payOrderApp.DownLoad(queryJson, companyId);
        }
    }
}