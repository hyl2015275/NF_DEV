/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class MeterChargingApp
    {
        private readonly IMeterChargingRepository _service = new MeterChargingRepository();
        public MeterChargingEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public bool IsUsePrice(string priceId)
        {
            return _service.IQueryable(x => x.F_PriceModel == priceId).Any();
        }
        public decimal UpdateBalance(string archiveId, decimal money)
        {
            var meter = _service.FindEntity(archiveId);
            if (meter == null) return 0;
            meter.F_Balance = (meter.F_Balance ?? 0) - money;
            _service.Update(meter);
            return meter.F_Balance ?? 0;
        }

        public decimal SetBalance(string archiveId, decimal money)
        {
            var meter = _service.FindEntity(archiveId);
            if (meter == null) return 0;
            meter.F_Balance = money;
            _service.Update(meter);
            return meter.F_Balance ?? 0;
        }

        public void UpdateForm(MeterChargingEntity meterChargingEntity)
        {
            _service.Update(meterChargingEntity);
        }
    }
}
