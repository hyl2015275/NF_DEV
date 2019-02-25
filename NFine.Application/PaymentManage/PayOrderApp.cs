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
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;
using NFine.Repository.DataStatistics;
using NFine.Repository.PaymentManage;

namespace NFine.Application.PaymentManage
{
    public class PayOrderApp
    {
        private readonly IPayOrderRepository _service = new PayOrderRepository();
        private readonly IRechargeReportRepository _reportService = new RechargeReportRepository();
        private readonly IPayOrderViewRepository _viewService = new PayOrderViewRepository();
        public List<PayOrderViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<PayOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                //充值类型
                expression = expression.And(t => t.F_PayType == type);
            }
            if (!queryParam["price"].IsEmpty())
            {
                var price = queryParam["price"].ToString();
                //计费方式
                expression = expression.And(t => t.F_PriceModel == price);
            }
            if (!queryParam["state"].IsEmpty())
            {
                var state = queryParam["state"].ToString();
                //充值类型
                expression = expression.And(t => t.F_State == state);
            }
            else
            {
                var state = ((int)PayStateEnum.Build).ToString();
                expression = expression.And(t => t.F_State != state);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_PaymentTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_PaymentTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }

        public decimal GetSum(string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<PayOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                //充值类型
                expression = expression.And(t => t.F_PayType == type);
            }
            if (!queryParam["price"].IsEmpty())
            {
                var price = queryParam["price"].ToString();
                //计费方式
                expression = expression.And(t => t.F_PriceModel == price);
            }
            if (!queryParam["state"].IsEmpty())
            {
                var state = queryParam["state"].ToString();
                //充值类型
                expression = expression.And(t => t.F_State == state);
            }
            else
            {
                var state = ((int)PayStateEnum.Build).ToString();
                expression = expression.And(t => t.F_State != state);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_PaymentTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_PaymentTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.IQueryable(expression).Sum(x => x.F_Money ?? 0);
        }
        public List<PayOrderViewModel> GetList(DateTime begintime, DateTime endtime, string companyId = "")
        {
            var expression = ExtLinq.True<PayOrderViewModel>();
            if (begintime != null)
            {
                expression = expression.And(t => t.F_PaymentTime >= begintime);
            }
            if (endtime != null)
            {
                endtime = endtime.AddDays(1);
                expression = expression.And(t => t.F_PaymentTime < endtime);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.IQueryable(expression).ToList();
        }
        public List<PayOrderViewModel> GetListByMeterCode(string meterCode)
        {
            return _viewService.IQueryable(x => x.F_MeterCode == meterCode && x.F_State == "1").ToList();
        }
        public PayOrderEntity GetFormByOrderNumber(string orderNumber)
        {
            return _service.FindEntity(x => x.F_OrderNumber == orderNumber && x.F_DeleteMark != true);
        }
        public PayOrderViewModel GetForm(string keyValue)
        {
            return _viewService.FindEntity(keyValue);
        }
        public void SubmitForm(PayOrderEntity payOrderEntity, string keyValue = "")
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                _service.Insert(payOrderEntity);
            }
            else
            {
                _service.Update(payOrderEntity);
            }
        }
        public void SubmitForm(PayOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity)
        {
            payOrderEntity.Create();
            CheckEntity(payOrderEntity);
            _service.SubmitForm(payOrderEntity, meterChargingEntity);
        }
        public void SubmitForm(PayOrderEntity payOrderEntity, MeterChargingEntity meterChargingEntity, ReadTaskEntity readTaskEntity)
        {
            payOrderEntity.Create();
            CheckEntity(payOrderEntity);
            _service.SubmitForm(payOrderEntity, meterChargingEntity, readTaskEntity);
        }
        public void CheckEntity(PayOrderEntity payOrderEntity)
        {
            var wait = ((int)PayStateEnum.Wait).ToString();
            var write = ((int)PayStateEnum.Write).ToString();
            if (_service.IQueryable(x => x.F_ArchiveId == payOrderEntity.F_ArchiveId && (x.F_State == wait || x.F_State == write)).Any())
                throw new Exception("当前表计存在未到账或未圈存充值记录");
        }
        public PayOrderEntity FindEntity(string deviceId)
        {
            var wait = ((int)PayStateEnum.Wait).ToString();
            return _viewService.FindEntity(x => x.F_MeterCode == deviceId && x.F_State == wait);
        }
        public int GetCountTotalByPayType(string payType, string companyId)
        {
            var state = ((int)PayStateEnum.Finish).ToString();
            return _service.IQueryable(x => x.F_PayType == payType && x.F_OwnerId == companyId).Count(x => x.F_State == state);
        }
        public decimal GetSumToal(string companyId)
        {
            var state = ((int)PayStateEnum.Finish).ToString();
            return _service.IQueryable(x => x.F_OwnerId == companyId && x.F_State == state).ToList().Sum(x => x.F_Money ?? 0);
        }
        public bool CheckState(string deviceId)
        {
            return _viewService.IQueryable(x => x.F_MeterCode == deviceId && x.F_State != "1").Any();
        }
        public int GetPayOrderCount(string deviceId)
        {
            return _viewService.IQueryable(x => x.F_MeterCode == deviceId && x.F_DeleteMark != true).Count();
        }
        public PayOrderEntity GetLastPayOrder(string deviceId)
        {
            return
                _viewService.IQueryable(x => x.F_MeterCode == deviceId && x.F_DeleteMark != true && x.F_State != "1")
                    .OrderByDescending(x => x.F_CreatorTime)
                    .FirstOrDefault();
        }
        public void DownLoad(string queryJson, string companyId)
        {
            var dataItems = (Dictionary<string, object>)new ItemsDetailApp().GetDataItemList();
            var expression = ExtLinq.True<PayOrderViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                //充值类型
                expression = expression.And(t => t.F_PayType == type);
            }
            if (!queryParam["state"].IsEmpty())
            {
                var state = queryParam["state"].ToString();
                //充值类型
                expression = expression.And(t => t.F_State == state);
            }
            else
            {
                var state = ((int)PayStateEnum.Build).ToString();
                expression = expression.And(t => t.F_State != state);
            }
            if (!queryParam["begintime"].IsEmpty())
            {
                var begintime = queryParam["begintime"].ToDate();
                //开始时间
                expression = expression.And(t => t.F_PaymentTime >= begintime);
            }
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = queryParam["endtime"].ToDate().AddDays(1);
                //结束时间
                expression = expression.And(t => t.F_PaymentTime < endtime);
            }
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //用户卡号、表计编码、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_MeterCode.Contains(keyword) || t.F_CustomerName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_State == ((int)PayStateEnum.Finish).ToString());
            var recordList = _viewService.IQueryable(expression).OrderByDescending(x => x.F_CreatorTime);
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("订单号"),
                new DataColumn("用户卡号"),
                new DataColumn("表计编码"),
                new DataColumn("缴费金额"),
                new DataColumn("缴费方式"),
                new DataColumn("本次余额"),
                new DataColumn("客户名称"),
                new DataColumn("经办人"),
                new DataColumn("缴费时间"),
                new DataColumn("外部订单号"),
            });
            foreach (var en in recordList)
            {
                var row = dt.NewRow();
                row[0] = en.F_OrderNumber;
                row[1] = en.F_UserCard;
                row[2] = en.F_MeterCode;
                row[3] = en.F_Money;
                if (dataItems != null)
                {
                    row[4] = ((Dictionary<string, string>)dataItems["PayType"])[en.F_PayType];
                }
                else
                {

                    row[4] = "";
                }
                row[5] = en.F_Balance;
                row[6] = en.F_CustomerName;
                row[7] = en.F_CreatorUserName;
                row[8] = en.F_PaymentTime;
                row[9] = en.F_OutOrder;
                dt.Rows.Add(row);
            }
            NPOIExcel.ExportByWeb(dt, "缴费记录", "缴费记录.xls");
        }
        public List<RechargeReportViewModel> GetDailyData(DateTime reportDay, string companyId)
        {
            var startTime = reportDay;
            var endTime = reportDay.AddDays(1);
            var paymentList =
                _viewService.IQueryable(
                    x => x.F_OwnerId == companyId && x.F_PaymentTime > startTime && x.F_PaymentTime < endTime && x.F_State == "1").ToList();
            var data = paymentList.Select(x => new RechargeReportViewModel
            {
                F_PurchaseDate = x.F_PaymentTime.ToDateString(),
                F_PurchaseNumber = 1,
                F_AccumulativeTotal = x.F_PayTotal,
                F_AccumulativeAmount = x.F_Money ?? 0,
                F_Description = x.F_MeterCode,
            }).ToList();
            data.Add(new RechargeReportViewModel
            {
                F_PurchaseDate = "合计：",
                F_PurchaseNumber = data.Sum(x => x.F_PurchaseNumber),
                F_AccumulativeTotal = data.Sum(x => x.F_AccumulativeTotal),
                F_AccumulativeAmount = data.Sum(x => x.F_AccumulativeAmount),
            });
            return data;
        }
        public List<RechargeReportViewModel> GetMonthlyReport(DateTime reportDay, string companyId)
        {
            var month = reportDay.ToString("yyyy-MM");
            var data = _reportService.IQueryable(x => x.F_OwnerId == companyId && x.F_PurchaseDate.StartsWith(month)).ToList();
            data.Add(new RechargeReportViewModel
            {
                F_PurchaseDate = "合计：",
                F_PurchaseNumber = data.Sum(x => x.F_PurchaseNumber),
                F_AccumulativeTotal = data.Sum(x => x.F_AccumulativeTotal),
                F_AccumulativeAmount = data.Sum(x => x.F_AccumulativeAmount),
            });
            return data;
        }
        public List<RechargeReportViewModel> GetAnnualReport(DateTime reportDay, string companyId)
        {
            var year = reportDay.ToString("yyyy");
            var paymentList = _reportService.IQueryable(x => x.F_OwnerId == companyId && x.F_Month.StartsWith(year)).ToList();
            var data = paymentList.GroupBy(m => m.F_Month).Select(x => new RechargeReportViewModel
            {
                F_PurchaseDate = x.Key,
                F_PurchaseNumber = x.Sum(s => s.F_PurchaseNumber),
                F_AccumulativeTotal = x.Sum(s => s.F_AccumulativeTotal),
                F_AccumulativeAmount = x.Sum(s => s.F_AccumulativeAmount),
            }).ToList();
            data.Add(new RechargeReportViewModel
            {
                F_PurchaseDate = "合计：",
                F_PurchaseNumber = data.Sum(x => x.F_PurchaseNumber),
                F_AccumulativeTotal = data.Sum(x => x.F_AccumulativeTotal),
                F_AccumulativeAmount = data.Sum(x => x.F_AccumulativeAmount),
            });
            return data;
        }
    }
}