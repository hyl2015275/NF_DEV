/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.MeterReading;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class UnusualController : ControllerBase
    {
        private readonly ReadRecordApp _readRecordApp = new ReadRecordApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _readRecordApp.GetNonOnlineList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var recordList = _readRecordApp.GetNonOnlineList(null, queryJson, companyId);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("表类型"),
                new DataColumn("生产厂商"),
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),
                new DataColumn("客户名称"),
                new DataColumn("上次底数"),
                new DataColumn("上次上线"),
                new DataColumn("联系方式"),
                new DataColumn("安装地址"),
                new DataColumn("备注"),
            });
            foreach (var en in recordList)
            {
                var row = dt.NewRow();
                if (dataItems != null)
                {
                    row[0] = ((Dictionary<string, string>)dataItems["DeviceType"])[en.F_MeterType];
                }
                else
                {
                    row[0] = "";
                }
                row[1] = en.F_Factor;
                row[2] = en.F_UserCard;
                row[3] = en.F_MeterCode;
                row[4] = en.F_CustomerName;
                row[5] = en.F_TotalDosage == null ? "无" : ((decimal)en.F_TotalDosage).ToString("0.00");
                row[6] = en.F_LastReadTime == null ? "无" : ((DateTime)en.F_LastReadTime).ToString("yyyy-MM-dd HH:mm:ss");
                row[7] = en.F_MobilePhone;
                row[8] = en.F_CustomerAddress;
                row[9] = en.F_Description;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "上线统计", "上线统计.xls");
        }
    }
}