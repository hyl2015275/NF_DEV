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
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NFine.Application.MeterReading;
using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Excel;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Domain.ViewModel.MeterReading;
using NFine.Repository.ArchiveManage;
using NFine.Repository.DataStatistics;
using NFine.Repository.MeterReading;

namespace NFine.Application.DataStatistics
{
    public class StatisticsApp
    {
        private readonly IStatisticsRepository _viewService = new StatisticsRepository();
        private readonly ICustomerStatisticsRepository _customerViewService = new CustomerStatisticsRepository();
        private readonly IReadRecordViewRepository _readRecordViewService = new ReadRecordViewRepository();
        private readonly IMeterRepository _meterService = new MeterRepository();

        public List<StatisticsViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<StatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_DateTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_DateTime < endtime);
            }
            if (!queryParam["keyValue"].IsEmpty())
            {
                var meterCode = queryParam["keyValue"].ToString();
                //表计编码
                expression = expression.And(t => t.F_MeterCode == meterCode);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }

        public List<StatisticsViewModel> GetList(string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<StatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_DateTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_DateTime < endtime);
            }
            if (!queryParam["keyValue"].IsEmpty())
            {
                var meterCode = queryParam["keyValue"].ToString();
                //表计编码
                expression = expression.And(t => t.F_MeterCode == meterCode);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.IQueryable(expression).ToList();
        }

        public List<CustomerStatisticsViewModel> GetCustomerStatisticsList(Pagination pagination, string queryJson, string companyId = "", string keyword = "")
        {
            var expression = ExtLinq.True<CustomerStatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["month"].IsEmpty())
            {
                var month = queryParam["month"].ToString();
                //月份
                expression = expression.And(t => t.F_Month == month);
            }
            if (!queryParam["meterType"].IsEmpty())
            {
                var meterTypeStr = queryParam["meterType"].ToString();
                var meterType = new ItemsDetailApp().GetKeyByValue("DeviceType", meterTypeStr);
                expression = expression.And(t => t.F_MeterType == meterType);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _customerViewService.FindList(expression, pagination);
        }

        public List<CustomerStatisticsViewModel> GetCustomerStatisticsList(string month, string companyId = "")
        {
            if (!string.IsNullOrEmpty(companyId))
                return _customerViewService.IQueryable(x => x.F_OwnerId == companyId && x.F_Month == month).ToList();
            else
                return _customerViewService.IQueryable(x => x.F_Month == month).ToList();
        }
        public List<CustomerStatisticsViewModel> DownLoad(string queryJson, string companyId = "", string keyword = "")
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var expression = ExtLinq.True<CustomerStatisticsViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["month"].IsEmpty())
            {
                var month = queryParam["month"].ToString();
                //月份
                expression = expression.And(t => t.F_Month == month);
            }
            if (!queryParam["meterType"].IsEmpty())
            {
                var meterTypeStr = queryParam["meterType"].ToString();
                var meterType = ((Dictionary<string, string>)dataItems["DeviceType"]).Where(q => q.Value != null && q.Value == meterTypeStr).Select(q => q.Key).FirstOrDefault();
                expression = expression.And(t => t.F_MeterType == meterType);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_CustomerAddress.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var recordList =
                _customerViewService.IQueryable(expression)
                    .OrderBy(x => x.F_CustomerName)
                    .ThenBy(x => x.F_MeterCode)
                    .ToList();
            return recordList;
        }

        public List<DosageStatisticsViewModel> GetDosageList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ReadRecordViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["businessID"].IsEmpty())
            {
                var businessID = queryParam["businessID"].ToString();
                //营业所号
                expression = expression.And(t => t.F_Description.Contains("\"BusinessID\":\"" + businessID + "\""));
            }
            if (!queryParam["areaID"].IsEmpty())
            {
                var areaID = queryParam["areaID"].ToString();
                //小区编号
                expression = expression.And(t => t.F_Description.Contains("\"AreaID\":\"" + areaID + "\""));
            }
            if (!queryParam["bookID"].IsEmpty())
            {
                var bookID = queryParam["bookID"].ToString();
                //册本号
                expression = expression.And(t => t.F_Description.Contains("\"BookID\":\"" + bookID + "\""));
            }
            if (!queryParam["inTabManIDs"].IsEmpty())
            {
                var inTabManIDs = queryParam["inTabManIDs"].ToString();
                //抄表员编号
                expression = expression.And(t => t.F_UserCard.Contains(inTabManIDs));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var tempData =
                _readRecordViewService.IQueryable(expression)
                    .GroupBy(x => new { x.F_ArchiveId, x.F_CustomerName, x.F_MeterCode, x.F_MeterType, x.F_CustomerAddress, x.F_MobilePhone, x.F_UserCard, x.F_Description })
                    .Select(
                        x => new DosageStatisticsViewModel
                        {
                            F_Id = x.Key.F_ArchiveId,
                            F_ThisBill = x.Sum(s => s.F_ThisBill ?? 0),
                            F_ThisDosage = x.Sum(s => s.F_ThisDosage ?? 0),
                            F_ThisRecord = x.Max(s => s.F_TotalDosage ?? 0),
                            F_LastRecord = x.Min(s => s.F_LastDosage ?? 0),
                            F_UserCard = x.Key.F_UserCard,
                            F_CustomerName = x.Key.F_CustomerName,
                            F_MeterCode = x.Key.F_MeterCode,
                            F_MeterType = x.Key.F_MeterType,
                            F_CustomerAddress = x.Key.F_CustomerAddress,
                            F_MobilePhone = x.Key.F_MobilePhone,
                            F_UnitPrice = x.Average(s => s.F_UnitPrice ?? 0),
                            F_Description = x.Key.F_Description
                        }
                    ).AsQueryable();
            bool isAsc = pagination.sord.ToLower() == "asc";
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC";
                }
                var parameter = Expression.Parameter(typeof(DosageStatisticsViewModel), "t");
                var property = typeof(DosageStatisticsViewModel).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(DosageStatisticsViewModel), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<DosageStatisticsViewModel>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).AsQueryable();
            return tempData.ToList();
        }

        public List<DosageStatisticsViewModel> DownLoad(string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ReadRecordViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_ReadTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_ReadTime < endtime);
            }
            if (!queryParam["device"].IsEmpty())
            {
                var device = queryParam["device"].ToString();
                //设备类型
                expression = expression.And(t => t.F_MeterType == device);
            }
            if (!queryParam["businessID"].IsEmpty())
            {
                var businessID = queryParam["businessID"].ToString();
                //营业所号
                expression = expression.And(t => t.F_Description.Contains("\"BusinessID\":\"" + businessID + "\""));
            }
            if (!queryParam["areaID"].IsEmpty())
            {
                var areaID = queryParam["areaID"].ToString();
                //小区编号
                expression = expression.And(t => t.F_Description.Contains("\"AreaID\":\"" + areaID + "\""));
            }
            if (!queryParam["bookID"].IsEmpty())
            {
                var bookID = queryParam["bookID"].ToString();
                //册本号
                expression = expression.And(t => t.F_Description.Contains("\"BookID\":\"" + bookID + "\""));
            }
            if (!queryParam["inTabManIDs"].IsEmpty())
            {
                var inTabManIDs = queryParam["inTabManIDs"].ToString();
                //抄表员编号
                expression = expression.And(t => t.F_UserCard.Contains(inTabManIDs));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            var tempData =
                _readRecordViewService.IQueryable(expression)
                    .GroupBy(x => new { x.F_ArchiveId, x.F_CustomerName, x.F_MeterCode, x.F_MeterType, x.F_CustomerAddress, x.F_MobilePhone, x.F_UserCard, x.F_Description })
                    .Select(
                        x => new DosageStatisticsViewModel
                        {
                            F_Id = x.Key.F_ArchiveId,
                            F_ThisBill = x.Sum(s => s.F_ThisBill ?? 0),
                            F_ThisDosage = x.Sum(s => s.F_ThisDosage ?? 0),
                            F_ThisRecord = x.Max(s => s.F_TotalDosage ?? 0),
                            F_LastRecord = x.Min(s => s.F_LastDosage ?? 0),
                            F_UserCard = x.Key.F_UserCard,
                            F_CustomerName = x.Key.F_CustomerName,
                            F_MeterCode = x.Key.F_MeterCode,
                            F_MeterType = x.Key.F_MeterType,
                            F_CustomerAddress = x.Key.F_CustomerAddress,
                            F_MobilePhone = x.Key.F_MobilePhone,
                            F_UnitPrice = x.Average(s => s.F_UnitPrice ?? 0),
                            F_Description = x.Key.F_Description
                        }
                    ).AsQueryable();
            return tempData.ToList();
        }
    }
}