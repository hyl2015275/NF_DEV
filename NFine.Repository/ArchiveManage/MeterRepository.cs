/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System;
using System.Linq;
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Repository.ArchiveManage
{
    public class MeterRepository : RepositoryBase<MeterEntity>, IMeterRepository
    {
        public void SubmitForm(MeterEntity meterEntity, MeterChargingEntity meterChargingEntity, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    meterEntity.Modify(keyValue);
                    db.Update(meterEntity);
                    meterChargingEntity.F_ArchiveId = meterEntity.F_Id;
                    db.Update(meterChargingEntity);
                }
                else
                {
                    meterEntity.Create();
                    db.Insert(meterEntity);
                    meterChargingEntity.F_ArchiveId = meterEntity.F_Id;
                    db.Insert(meterChargingEntity);
                }
                db.Commit();
            }
        }
    }

    public class MeterViewRepository : RepositoryBase<MeterViewModel>, IMeterViewRepository
    {

    }
}