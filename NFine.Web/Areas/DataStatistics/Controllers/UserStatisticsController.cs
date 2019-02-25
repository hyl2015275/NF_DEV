/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.DataStatistics;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.ViewModel.DataStatistics;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NFine.Web.Areas.DataStatistics.Controllers
{
    public class UserStatisticsController : ControllerBase
    {
        private readonly StatisticsApp _statisticsApp = new StatisticsApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _statisticsApp.GetList(pagination, queryJson, companyId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalaryChart(string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var statisticsList = _statisticsApp.GetList(queryJson, companyId);
            var data = new
            {
                labels = statisticsList.OrderBy(x => x.F_Date).Select(x => x.F_Date).ToArray(),
                data = new[]
                {
                    statisticsList.OrderBy(x=>x.F_Date).Select(x => x.F_SumDosage.ToString(CultureInfo.InvariantCulture)).ToArray()
                }
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCustomerStatisticsGridJson(Pagination pagination, string queryJson, string keyword = "")
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _statisticsApp.GetCustomerStatisticsList(pagination, queryJson, companyId, keyword),
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
        public virtual void DownLoad(string queryJson, string keyword = "")
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            queryJson = HttpUtility.UrlDecode(queryJson, System.Text.Encoding.UTF8);
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var recordList = _statisticsApp.DownLoad(queryJson, companyId, keyword);
            var queryParam = queryJson.ToJObject();
            //创业园电表数据导出
            if (companyId == "ff92def0-dabe-4878-915b-1b8cd0560ce4" && !queryParam["month"].IsEmpty() &&
                !queryParam["meterType"].IsEmpty() && queryParam["meterType"].ToString() == "电表")
            {
                var month = queryParam["month"].ToString();
                NPOIExcel.ExportByWeb(PioneerPark(month, recordList), month + "用电及退补情况表.xls");
            }
            else
            {
                var dt = new DataTable();
                dt.Columns.AddRange(new[]
            {
                new DataColumn("表类型"),
                new DataColumn("客户名称"),
                new DataColumn("表计编码"),
                new DataColumn("上月抄表"),
                new DataColumn("本月抄表"),

                new DataColumn("倍率"),

                new DataColumn("本月用量"),

                new DataColumn("单位"),
                new DataColumn("执行价格"),

                new DataColumn("本月计费"),
                new DataColumn("安装地址"),
            });
                foreach (var en in recordList)
                {
                    var row = dt.NewRow();
                    if (dataItems != null)
                    {
                        row[0] = ((Dictionary<string, string>)dataItems["DeviceType"])[en.F_MeterType];
                        row[7] = ((Dictionary<string, string>)dataItems["DeviceUnit"])[en.F_MeterType];
                    }
                    else
                    {
                        row[0] = "";
                        row[7] = "";
                    }
                    row[1] = en.F_CustomerName;
                    row[2] = en.F_MeterCode;
                    row[3] = en.F_LastMonthRecord;
                    row[4] = en.F_ThisMonthRecord;

                    row[5] = en.F_MeterRate ?? 1;

                    row[6] = en.F_ThisMonthDosage;


                    row[8] = en.F_UnitPrice;

                    row[9] = en.F_ThisMonthBill;
                    row[10] = en.F_CustomerAddress;
                    dt.Rows.Add(row);
                }
                NPOIExcel.ExportByWeb(dt, "用量详情", "用量详情.xls");
            }
        }
        /// <summary>
        /// 创业园多退少补
        /// </summary>
        /// <param name="month">统计月</param>
        /// <param name="recordList">抄表记录</param>
        /// <returns></returns>
        private MemoryStream PioneerPark(string month, List<CustomerStatisticsViewModel> recordList)
        {
            var rowNumber = 19;
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Areas/DataStatistics/Template/PioneerPark.xls"); //路径
            IWorkbook wk = NPOIExcel.ReadTemplateFile(filePath);
            var sheet = wk.GetSheetAt(0);
            var style = wk.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            var style2 = wk.CreateCellStyle();
            style2.BorderBottom = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderTop = BorderStyle.Thin;
            style2.BottomBorderColor = HSSFColor.Black.Index;
            style2.LeftBorderColor = HSSFColor.Black.Index;
            style2.RightBorderColor = HSSFColor.Black.Index;
            style2.TopBorderColor = HSSFColor.Black.Index;
            style2.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            //设置标题
            var title = sheet.GetRow(0).GetCell(0).StringCellValue;
            title = title.Replace("%%year%%", new string(month.Take(4).ToArray())).Replace("%%month%%", new string(month.Skip(4).Take(2).ToArray()));
            sheet.GetRow(0).GetCell(0).SetCellValue(title);
            //循环数据
            var companyData = recordList.GroupBy(x => x.F_CustomerName).OrderBy(x => x.Key).ToList();
            foreach (var item in companyData)
            {
                var startRows = rowNumber;
                foreach (var t in item)
                {
                    var sheetRow = rowNumber - 1;
                    var serialNumber = rowNumber - 18;
                    sheet.GetRow(sheetRow).GetCell(0).SetCellValue(serialNumber);//序号
                    sheet.GetRow(sheetRow).GetCell(0).CellStyle = style;
                    sheet.GetRow(sheetRow).GetCell(1).SetCellValue(t.F_CustomerName);//单位
                    sheet.GetRow(sheetRow).GetCell(1).CellStyle = style;
                    sheet.GetRow(sheetRow).GetCell(2).SetCellValue(t.F_CustomerAddress);//用电地址
                    sheet.GetRow(sheetRow).GetCell(2).CellStyle = style;
                    sheet.GetRow(sheetRow).GetCell(3).SetCellValue(Math.Round((double)t.F_LastMonthRecord, 2));//本月用电月初抄表数
                    sheet.GetRow(sheetRow).GetCell(4).SetCellValue(Math.Round((double)t.F_ThisMonthRecord, 2));//本月用电月末抄表数
                    sheet.GetRow(sheetRow).GetCell(5).SetCellFormula("E" + rowNumber + "-D" + rowNumber);//本月用电量
                    sheet.GetRow(sheetRow).GetCell(6).SetCellFormula("F" + rowNumber + "*I" + rowNumber);//系统计费金额
                    sheet.GetRow(sheetRow).GetCell(7).SetCellFormula("SUM(F" + startRows + ":F" + (startRows + item.Count() - 1) + ")");//本月单位合户用电合计
                    sheet.GetRow(sheetRow).GetCell(8).SetCellValue(double.Parse("1.2"));//系统实收单价
                    sheet.GetRow(sheetRow).GetCell(9).SetCellValue(Math.Round((double)t.F_ThisMonthBalance, 3));//月末系统余额
                    sheet.GetRow(sheetRow).GetCell(10).SetCellFormula("F" + rowNumber + "*D3");//按实际电价计费金额
                    sheet.GetRow(sheetRow).GetCell(11).SetCellFormula("SUM(K" + startRows + ":K" + (startRows + item.Count() - 1) + ")");//实际电价计费合计
                    sheet.GetRow(sheetRow).GetCell(12).SetCellFormula("K" + rowNumber + "-G" + rowNumber);//本月每表退/补差额
                    sheet.GetRow(sheetRow).GetCell(13).SetCellFormula("SUM(M" + startRows + ":M" + (startRows + item.Count() - 1) + ")");//退补差额单位合户合计
                    for (var i = 3; i <= 13; i++)
                    {
                        sheet.GetRow(sheetRow).GetCell(i).CellStyle = style2;
                    }
                    rowNumber++;
                }
                sheet.AddMergedRegion(new CellRangeAddress(startRows - 1, startRows + item.Count() - 2, 1, 1));//名称合并
                sheet.AddMergedRegion(new CellRangeAddress(startRows - 1, startRows + item.Count() - 2, 7, 7));//用电合并
                sheet.AddMergedRegion(new CellRangeAddress(startRows - 1, startRows + item.Count() - 2, 11, 11));//计费合并
                sheet.AddMergedRegion(new CellRangeAddress(startRows - 1, startRows + item.Count() - 2, 13, 13));//退补合并
            }
            //汇总统计
            sheet.GetRow(2).GetCell(5).SetCellFormula("SUM(F19" + ":F" + (rowNumber - 1) + ")");//本月系统总用量
            sheet.GetRow(2).GetCell(6).SetCellFormula("SUM(G19" + ":G" + (rowNumber - 1) + ")");//系统计费金额合计
            sheet.GetRow(2).GetCell(7).SetCellFormula("F3");//本月用电量总合计
            sheet.GetRow(2).GetCell(9).SetCellFormula("SUM(J19" + ":J" + (rowNumber - 1) + ")");//月末系统余额合计
            sheet.GetRow(2).GetCell(10).SetCellFormula("SUM(K19" + ":K" + (rowNumber - 1) + ")");//实际电价金额合计
            sheet.GetRow(2).GetCell(11).SetCellFormula("K3");//实际电价计费合户合计
            sheet.GetRow(2).GetCell(12).SetCellFormula("SUM(M19" + ":M" + (rowNumber - 1) + ")");//本月退/补差额合计
            sheet.GetRow(2).GetCell(13).SetCellFormula("M3");//本月退补差额单位合户合计
            sheet.ForceFormulaRecalculation = true;
            using (var ms = new MemoryStream())
            {
                wk.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
    }
}