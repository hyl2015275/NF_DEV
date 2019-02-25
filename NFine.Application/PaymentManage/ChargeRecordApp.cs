/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NFine.Application.ArchiveManage;
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.PaymentManage;
using NFine.Domain.ViewModel.PaymentManage;
using NFine.Repository.PaymentManage;

namespace NFine.Application.PaymentManage
{
    public class ChargeRecordApp
    {
        private readonly IChargeRecordRepository _service = new ChargeRecordRepository();
        private readonly IChargeRecordViewRepository _viewService = new ChargeRecordViewRepository();
        private readonly MeterChargingApp _meterChargingApp = new MeterChargingApp();
        private readonly MeterApp _meterApp = new MeterApp();
        private readonly PriceApp _priceApp = new PriceApp();
        private readonly ReadRecordApp _readRecordApp = new ReadRecordApp();
        public void ServiceSubmit(List<ReadRecordEntity> readList)
        {
            foreach (var item in readList)
            {
                //获取计费信息
                var meterCharging = _meterChargingApp.GetForm(item.F_ArchiveId);
                //获取当前周期开始时间
                var cycleStartTime = _priceApp.GetCycleStartTime(meterCharging.F_PriceModel);
                //获取当前周期开始时间前一天最后一条抄表记录
                var cycleDosage = cycleStartTime == DateTime.MinValue ? 0 : decimal.Parse(_readRecordApp.GetLastReadRecord(item.F_ArchiveId, cycleStartTime));
                //单位价格
                var uintPrice = _priceApp.GetPriceValue(meterCharging.F_PriceModel, ((item.F_TotalDosage ?? 0) - cycleDosage).ToString(CultureInfo.InvariantCulture));
                _service.Insert(new ChargeRecordEntity
                {
                    F_ThisBill = (item.F_ThisDosage ?? 0) * uintPrice,
                    F_ReadId = item.F_Id,
                    F_UnitPrice = uintPrice,
                    F_Description = "",
                    F_Balance = _meterChargingApp.UpdateBalance(item.F_ArchiveId, (item.F_ThisDosage ?? 0) * uintPrice),
                    F_ChargeTime = DateTime.Now
                });
            }
        }
        public List<ChargeRecordViewModel> GetList(Pagination pagination, string keyword, string companyId = "")
        {
            var expression = ExtLinq.True<ChargeRecordViewModel>();
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                //用户卡号、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_MeterCode.Contains(keyword));
            }
            return _viewService.FindList(expression, pagination);
        }
        public decimal GetSum(string keyword, string companyId = "")
        {
            var expression = ExtLinq.True<ChargeRecordViewModel>();
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                //用户卡号、客户姓名
                expression = expression.And(t => t.F_UserCard.Contains(keyword) || t.F_CustomerName.Contains(keyword) || t.F_MeterCode.Contains(keyword));
            }
            return _viewService.IQueryable(expression).Sum(x => x.F_ThisBill);
        }

        public decimal GetSumTotal(string companyId, DateTime? startTime = null, DateTime? endTime = null)
        {
            IQueryable<ChargeRecordViewModel> list;
            if (startTime != null && endTime != null)
            {
                list = _viewService.IQueryable(x => x.F_OwnerId == companyId&&x.F_ReadTime>=startTime&&x.F_ReadTime<endTime);
            }
            else {
                list = _viewService.IQueryable(x => x.F_OwnerId == companyId);
            }
            return list.Any() ? list.Sum(x => x.F_ThisBill) : 0;
        }
    }
}