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
using System.Text;
using System.Threading.Tasks;
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class PriceApp
    {
        private readonly IPriceRepository _service = new PriceRepository();
        private readonly PriceBaseApp _priceBaseApp = new PriceBaseApp();
        private readonly PriceDetailsApp _priceDetailsApp = new PriceDetailsApp();
        private readonly MeterChargingApp _meterChargingApp = new MeterChargingApp();
        public void SubmitForm(PriceEntity priceEntity, List<PriceBaseEntity> basesList, List<PriceDetailsEntity> detailsList, string keyValue)
        {
            if (basesList.Count == 1)
            {
                priceEntity.F_UnitPrice = (basesList.First().F_PriceValue + detailsList.Sum(x => x.F_Price)).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                priceEntity.F_UnitPrice = (basesList.OrderBy(x => x.F_PriceValue).First().F_PriceValue + detailsList.Sum(x => x.F_Price)).ToString(CultureInfo.InvariantCulture) + "-" +
                                          (basesList.OrderBy(x => x.F_PriceValue).Last().F_PriceValue + detailsList.Sum(x => x.F_Price)).ToString(CultureInfo.InvariantCulture);
            }
            _service.SubmitForm(priceEntity, basesList, detailsList, keyValue);
        }
        public List<PriceEntity> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<PriceEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(t => t.F_PriceName.Contains(keyword));
            }
            if (!queryParam["type"].IsEmpty())
            {
                string type = queryParam["type"].ToString();
                expression = expression.And(t => t.F_PriceType == type);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            expression = expression.And(t => t.F_DeleteMark == false || t.F_DeleteMark == null);
            return _service.FindList(expression, pagination);
        }
        public List<PriceEntity> GetList(bool isStart, string companyId)
        {
            var thisTime = DateTime.Now;
            var expression = ExtLinq.True<PriceEntity>();
            expression = isStart ? expression.And(t => t.F_StartTime <= thisTime) : expression.And(t => t.F_StartTime > thisTime);
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _service.IQueryable(expression).ToList();
        }
        public PriceEntity GetForm(string companyId, string priceName)
        {
            return _service.FindEntity(x => x.F_OwnerId == companyId && x.F_PriceName == priceName);
        }
        public PriceEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public PriceEntity GetDefault(string companyId)
        {
            return _service.FindEntity(x => x.F_OwnerId == companyId);
        }
        public void DeleteForm(string keyValue)
        {
            if (_meterChargingApp.IsUsePrice(keyValue))
            {
                throw new Exception("删除失败！当前价格方式已被使用。");
            }
            _service.DeleteForm(keyValue);
        }

        /// <summary>
        /// 获取单位价格
        /// </summary>
        /// <param name="priceId">执行价格序号</param>
        /// <param name="totalDosage">总用量</param>
        /// <returns>单位价格</returns>
        public decimal GetPriceValue(string priceId, string cycleDosage)
        {
            decimal priceValue = 0;
            var priceEntity = _service.FindEntity(priceId);
            switch (priceEntity.F_PriceType)
            {
                case "1"://普通计价
                    {
                        priceValue = decimal.Parse(priceEntity.F_UnitPrice);
                    }
                    break;
                case "2"://阶梯计价
                    {
                        decimal basePrice = 0;
                        var priceBaseList = _priceBaseApp.GetList(priceId);
                        foreach (var item in priceBaseList)
                        {
                            if (basePrice > 0) continue;
                            var maxValue = item.F_PriceName.Split('-')[1];
                            if (decimal.Parse(cycleDosage) <= decimal.Parse(maxValue))
                                basePrice = item.F_PriceValue;
                        }
                        var sumDeatailsPrice = _priceDetailsApp.GetList(priceId).Sum(x => x.F_Price);
                        priceValue = basePrice + sumDeatailsPrice;
                    }
                    break;
            }
            return priceValue;
        }

        public DateTime GetCycleStartTime(string priceId)
        {
            var priceEntity = _service.FindEntity(priceId);
            var cycleStartTime = priceEntity.F_StartTime;
            var now = DateTime.Now.Date;
            while (cycleStartTime < now)
            {
                switch (priceEntity.F_Cycle)
                {
                    case 2:
                        {
                            cycleStartTime = cycleStartTime.AddDays(7);
                        }
                        break;
                    case 3:
                        {
                            cycleStartTime = cycleStartTime.AddMonths(1);
                        }
                        break;
                    case 4:
                        {
                            cycleStartTime = cycleStartTime.AddMonths(3);
                        }
                        break;
                    case 5:
                        {
                            cycleStartTime = cycleStartTime.AddYears(1);
                        }
                        break;
                    default:
                        cycleStartTime = now;
                        break;
                }
            }
            switch (priceEntity.F_Cycle)
            {
                case 1:
                    {
                        cycleStartTime = cycleStartTime.AddDays(-1);
                    }
                    break;
                case 2:
                    {
                        cycleStartTime = cycleStartTime.AddDays(-7);
                    }
                    break;
                case 3:
                    {
                        cycleStartTime = cycleStartTime.AddMonths(-1);
                    }
                    break;
                case 4:
                    {
                        cycleStartTime = cycleStartTime.AddMonths(-3);
                    }
                    break;
                case 5:
                    {
                        cycleStartTime = cycleStartTime.AddYears(-1);
                    }
                    break;
                default:
                    cycleStartTime = DateTime.MinValue;
                    break;
            }
            return cycleStartTime;
        }
    }
}