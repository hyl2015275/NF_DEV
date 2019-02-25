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
using NFine.Application.MeterReading;
using NFine.Code;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;
using NFine.Repository.ArchiveManage;

namespace NFine.Application.ArchiveManage
{
    public class ChangeMeterApp
    {
        private readonly IChangeMeterRepository _service = new ChangeMeterRepository();
        private readonly IChangeMeterViewRepository _viewService = new ChangeMeterViewRepository();
        public List<ChangeMeterViewModel> GetList(Pagination pagination, string queryJson, string companyId = "")
        {
            var expression = ExtLinq.True<ChangeMeterViewModel>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(t => t.F_OldMeterCode.Contains(keyword) || t.F_MeterCode.Contains(keyword));
            }
            if (!queryParam["device"].IsEmpty())
            {
                string type = queryParam["device"].ToString();
                expression = expression.And(t => t.F_MeterType == type);
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                expression = expression.And(t => t.F_OwnerId == companyId);
            }
            return _viewService.FindList(expression, pagination);
        }
        public ChangeMeterViewModel GetFormByMeterKey(string keyValue)
        {
            var meter = new MeterApp().GetForm(keyValue);
            var lastRecord = new ReadRecordApp().GetLastReadRecord(meter.F_Id, DateTime.Now);
            return new ChangeMeterViewModel()
            {
                F_OldMeterCode = meter.F_MeterCode,
                F_MeterType = meter.F_MeterType,
                F_UserCard = meter.F_UserCard,
                F_MeterNumber = meter.F_MeterNumber,
                F_OwnerId = meter.F_OwnerId,
                F_Factor = meter.F_Factor,
                F_OldArchiveId = meter.F_Id,
                F_MobilePhone = meter.F_MobilePhone,
                F_CustomerAddress = meter.F_CustomerName,
                F_CustomerName = meter.F_CustomerName,
                F_TotalDosage = !string.IsNullOrEmpty(lastRecord) ? decimal.Parse(lastRecord) : 0
            };
        }

        public ChangeMeterEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }

        public void SubmitForm(ChangeMeterEntity changeMeterEntity, string meterCode)
        {
            CheckEntity(changeMeterEntity, meterCode);
            changeMeterEntity.F_ArchiveId = Common.GuId();
            var meter = new MeterApp().GetForm(changeMeterEntity.F_OldArchiveId);
            var meterCharging = new MeterChargingApp().GetForm(changeMeterEntity.F_OldArchiveId);
            ReadTaskEntity readTaskEntity = null;
            if (changeMeterEntity.F_AllowReplace == true)
            {
                readTaskEntity = new ReadTaskEntity
                {
                    F_CreatorTime = DateTime.Now,
                    F_Factor = meter.F_Factor,
                    F_Id = Common.GuId(),
                    F_MeterCode = meterCode,
                    F_MeterType = meter.F_MeterType,
                    F_Param = changeMeterEntity.F_BaseDosage.ToString(),
                    F_TaskType = (int)TaskTypeEnum.SetBaseDosage,
                    F_State = (int)TaskStateEnum.Wait,
                };
            }
            _service.SubmitForm(changeMeterEntity, meter, meterCharging, meterCode, readTaskEntity);
        }
        public void CheckEntity(ChangeMeterEntity meterEntity, string meterCode)
        {
            if (string.IsNullOrEmpty(meterCode))
                throw new Exception("表计编码不允许为空");
            if (meterEntity.F_BaseDosage == null && meterEntity.F_AllowReplace == true)
                throw new Exception("表计底数不允许为空");
            if (new MeterApp().GetFormByMeterCode(meterCode) != null)
                throw new Exception("表计编码" + meterCode + "已存在");
        }
    }
}