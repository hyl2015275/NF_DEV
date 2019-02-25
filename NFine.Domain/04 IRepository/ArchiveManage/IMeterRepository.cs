/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Domain.IRepository.ArchiveManage
{
    public interface IMeterRepository : IRepositoryBase<MeterEntity>
    {
        void SubmitForm(MeterEntity meterEntity, MeterChargingEntity meterChargingEntity, string keyValue);
    }
    public interface IMeterViewRepository : IRepositoryBase<MeterViewModel>
    {

    }
}
