using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Application.DataStatistics;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Web.Areas.DataStatistics.Controllers
{
    public class DosageStatisticsController : ControllerBase
    {
        private readonly StatisticsApp _statisticsApp = new StatisticsApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string queryJson)
        {
            var companyId = OperatorProvider.Provider.GetCurrent().CompanyId;
            var data = new
            {
                rows = _statisticsApp.GetDosageList(pagination, queryJson, companyId),
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
            var recordList = _statisticsApp.DownLoad(queryJson, companyId);
            var queryParam = queryJson.ToJObject();
            if (companyId == "030eca51-483a-47b6-ad46-b6164278ebcb")//张家口水司
            {
                var endtime = queryParam["endtime"].ToDate();
                var dt = new DataTable();
                dt.Columns.AddRange(new[]
                {
                    new DataColumn("UserID"),
                    new DataColumn("UserName"),
                    new DataColumn("Address"),
                    new DataColumn("Tel"),
                    new DataColumn("WaterTagID"),
                    new DataColumn("LINENO"),
                    new DataColumn("AreaID"),
                    new DataColumn("InTabManID"),
                    new DataColumn("AMTabNum"),
                    new DataColumn("NMTabNum"),
                    new DataColumn("WaterMete"),
                    new DataColumn("WaterPrice"),
                    new DataColumn("CBtDate"),
                });
                foreach (var en in recordList)
                {
                    var row = dt.NewRow();
                    var customPara = en.F_Description.ToObject<MeterCustomParameter>();
                    row[0] = en.F_UserCard;
                    row[1] = en.F_CustomerName;
                    row[2] = en.F_CustomerAddress;
                    row[3] = en.F_MobilePhone;
                    row[4] = en.F_MeterCode;
                    row[5] = customPara == null ? "" : customPara.LINENO;
                    row[6] = customPara == null ? "" : customPara.AreaID;
                    row[7] = customPara == null ? "" : customPara.InTabManIDs;
                    row[8] = int.Parse(en.F_LastRecord.ToString("#0"));
                    row[9] = int.Parse(en.F_ThisRecord.ToString("#0"));
                    row[10] = int.Parse(en.F_ThisRecord.ToString("#0")) - int.Parse(en.F_LastRecord.ToString("#0"));
                    row[11] = en.F_UnitPrice.ToString("#0.0");
                    row[12] = endtime.ToString("yyyy-MM-dd");
                    dt.Rows.Add(row);
                }
                var fileName = "11" + endtime.ToString("yyyyMM") + "S.xls";
                NPOIExcel.ExportByWeb(dt, fileName, fileName);
            }
            else
            {
                var dt = new DataTable();
                dt.Columns.AddRange(new[]
                {
                    new DataColumn("表类型"),
                    new DataColumn("用户卡号"),
                    new DataColumn("表计编码"),
                    new DataColumn("客户名称"),
                    new DataColumn("安装位置"),
                    new DataColumn("联系方式"),
                    new DataColumn("期初数"),
                    new DataColumn("期末数"),
                    new DataColumn("总用量"),
                    new DataColumn("单价"),
                    new DataColumn("总计费"),
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
                    row[1] = en.F_UserCard;
                    row[2] = en.F_MeterCode;
                    row[3] = en.F_CustomerName;
                    row[4] = en.F_CustomerAddress;
                    row[5] = en.F_MobilePhone;
                    row[6] = en.F_LastRecord;
                    row[7] = en.F_ThisRecord;
                    row[8] = en.F_ThisDosage;
                    row[9] = en.F_UnitPrice;
                    row[10] = en.F_ThisBill;
                    dt.Rows.Add(row);
                }
                NPOIExcel.ExportByWeb(dt, "用量统计", "用量统计.xls");
            }
        }
    }
}