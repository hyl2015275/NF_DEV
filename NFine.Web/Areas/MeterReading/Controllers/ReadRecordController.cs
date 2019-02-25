/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NFine.Application.MeterReading;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.Mbus;

namespace NFine.Web.Areas.MeterReading.Controllers
{
    public class ReadRecordController : ControllerBase
    {
        private readonly ReadRecordApp _readRecordApp = new ReadRecordApp();

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="pagination">分页排序</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="keyValue">任务编号</param>
        /// <returns>查询结果</returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson, string keyValue = "", string archiveId = "")
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
 
            var data = new
            {
                //rows = _readRecordApp.GetListByT_SQL(pagination, queryJson, companyId, keyValue, archiveId).OrderByDescending(o => o.F_ReadTime).OrderBy(o => Convert.ToInt32(o.F_Sort)),
                // 判断是否是港务局第三生活区
                rows = "462638b5-f026-44bd-8e2c-92a0ad46d6e0".Equals(companyId) ? _readRecordApp.GetListBySQL(pagination, queryJson, companyId).OrderBy(o => Convert.ToInt32(o.F_Sort)) : _readRecordApp.GetListByT_SQL(pagination, queryJson, companyId, keyValue, archiveId).OrderByDescending(o => o.F_ReadTime).OrderBy(o => Convert.ToInt32(o.F_Sort)),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };


            return Content(data.ToJson());
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="queryJson">查询条件</param>
        [HttpGet]
        public virtual void DownLoad(string queryJson)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var recordList = _readRecordApp.DownLoad(queryJson, companyId);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("表类型"),
                new DataColumn("抄表方式"),
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),

                new DataColumn("期初数"),
                new DataColumn("期末数"),
                new DataColumn("倍率"),

                new DataColumn("本次用量"),
                new DataColumn("累计用量"),

                new DataColumn("单位"),
                new DataColumn("执行价格"),
                new DataColumn("本次计费"),
                new DataColumn("本次余额"),

                new DataColumn("客户名称"),
                new DataColumn("联系方式"),
                new DataColumn("安装地址"),
                new DataColumn("抄表时间"),
            });
            foreach (var en in recordList)
            {
                var row = dt.NewRow();
                if (dataItems != null)
                {
                    row[0] = ((Dictionary<string, string>)dataItems["DeviceType"])[en.F_MeterType];
                    row[1] = ((Dictionary<string, string>)dataItems["ReadType"])[en.F_ReadType];
                    row[9] = ((Dictionary<string, string>)dataItems["DeviceUnit"])[en.F_MeterType];
                }
                else
                {
                    row[0] = "";
                    row[1] = "";
                    row[9] = "";
                }
                row[2] = en.F_UserCard;
                row[3] = en.F_MeterCode;

                row[4] = en.F_MeterRate == null || en.F_LastDosage == null
                    ? en.F_LastDosage
                    : (decimal)en.F_LastDosage / (decimal)en.F_MeterRate;
                row[5] = en.F_MeterRate == null || en.F_TotalDosage == null
                    ? en.F_TotalDosage
                    : (decimal)en.F_TotalDosage / (decimal)en.F_MeterRate;
                row[6] = en.F_MeterRate ?? 1;

                row[7] = en.F_ThisDosage;
                row[8] = en.F_TotalDosage;

                row[10] = en.F_UnitPrice;
                row[11] = en.F_ThisBill;
                row[12] = en.F_Balance;

                row[13] = en.F_CustomerName;
                row[14] = en.F_MobilePhone;
                row[15] = en.F_CustomerAddress;
                row[16] = en.F_ReadTime;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "抄表记录", "抄表记录.xls");
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = _readRecordApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
    }
}