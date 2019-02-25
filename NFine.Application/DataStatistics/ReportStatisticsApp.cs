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
using System.Text;
using System.Threading.Tasks;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Repository.DataStatistics;

namespace NFine.Application.DataStatistics
{
    public class ReportStatisticsApp
    {
        private readonly IReportStatisticsRepository _viewService = new ReportStatisticsRepository();
        public List<ReportStatisticsViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ReportStatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }

        public List<ReportStatisticsViewModel> GetFormByReportCyde(string reportCyde, string companyId = "")
        {
            if (!string.IsNullOrEmpty(companyId))
                return _viewService.IQueryable(x => x.F_OwnerId == companyId && x.F_ReportCycle == reportCyde).ToList();
            else
                return _viewService.IQueryable(x => x.F_ReportCycle == reportCyde).ToList();
        }
        public void DownLoad(string queryJson, string companyId)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var expression = ExtLinq.True<ReportStatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var recordList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_ReportCycle);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("表类型"),
                new DataColumn("周期"),
                new DataColumn("总用量"),
                new DataColumn("总计费"),
                new DataColumn("总收入"),
                new DataColumn("上线表数"),
                new DataColumn("抄表次数"),
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
                    row[0] = en.F_MeterType;
                }
                row[1] = en.F_ReportCycle;
                row[2] = en.F_SumDosage;
                row[3] = en.F_SumBill;
                row[4] = en.F_SumPay;
                row[5] = en.F_CountDevice;
                row[6] = en.F_CountRecord;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "用量统计", "用量统计.xls");
        }

        public decimal[][] GetMonthTotalByMeterType(string[] meterType, string companyId)
        {
            var datas = new decimal[meterType.Count()][];
            var now = DateTime.Now;
            var recordList = _viewService.IQueryable(x => x.F_OwnerId == companyId);
            for (int i = 0; i < meterType.Count(); i++)
            {
                var data = new decimal[12];
                var type = meterType[i];
                for (var j = 1; j <= 12; j++)
                {
                    var start = now.AddMonths(-j).ToString("yyyyMM");
                    var record = recordList.FirstOrDefault(x => x.F_MeterType == type && x.F_ReportCycle == start);
                    data[12 - j] = record == null ? 0 : record.F_SumDosage;
                }
                datas[i] = data;
            }
            return datas;
        }
    }
}