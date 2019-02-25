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
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Repository.ArchiveManage
{
    public class ChangeMeterRepository : RepositoryBase<ChangeMeterEntity>, IChangeMeterRepository
    {
        public void SubmitForm(ChangeMeterEntity changeMeterEntity, MeterEntity meterEntity, MeterChargingEntity meterChargingEntity, string meterCode, ReadTaskEntity readTaskEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                try
                {
                    changeMeterEntity.Create();
                    db.Insert(changeMeterEntity);
                    meterEntity.Remove();
                    db.Update(meterEntity);
                    db.Insert(new MeterEntity
                    {
                        F_CreatorTime = meterEntity.F_CreatorTime,
                        F_DeleteMark = false,
                        F_IDNumber = meterEntity.F_IDNumber,
                        F_DeleteTime = null,
                        F_DeleteUserId = null,
                        F_MeterCode = meterCode,
                        F_Id = changeMeterEntity.F_ArchiveId,
                        F_UserCard = meterEntity.F_UserCard,
                        F_OwnerId = meterEntity.F_OwnerId,
                        F_MobilePhone = meterEntity.F_MobilePhone,
                        F_CreatorUserId = meterEntity.F_CreatorUserId,
                        F_CustomerAddress = meterEntity.F_CustomerAddress,
                        F_CustomerName = meterEntity.F_CustomerName,
                        F_Description = meterEntity.F_Description,
                        F_Factor = meterEntity.F_Factor,
                        F_LastModifyTime = meterEntity.F_LastModifyTime,
                        F_LastModifyUserId = meterEntity.F_LastModifyUserId,
                        F_MeterName = meterEntity.F_MeterName,
                        F_MeterNumber = meterEntity.F_MeterNumber,
                        F_MeterRate = meterEntity.F_MeterRate,
                        F_MeterType = meterEntity.F_MeterType
                    });
                    var newMeterChargingEntity = meterChargingEntity;
                    newMeterChargingEntity.F_StartValue = changeMeterEntity.F_BaseDosage;
                    newMeterChargingEntity.F_ArchiveId = changeMeterEntity.F_ArchiveId;
                    db.Insert(newMeterChargingEntity);
                    if (readTaskEntity != null)
                    {
                        readTaskEntity.F_WorkId = changeMeterEntity.F_Id;
                        db.Insert(readTaskEntity);
                    }
                    db.Commit();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }

    public class ChangeMeterViewRepository : RepositoryBase<ChangeMeterViewModel>, IChangeMeterViewRepository
    {

    }
}