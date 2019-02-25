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
using NFine.Data;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enum;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Repository.MeterReading
{
    public class ReadRecordRepository : RepositoryBase<ReadRecordEntity>, IReadRecordRepository
    {

        public void SubmitService(ReadRecordEntity readRecordEntity, ChargeRecordEntity chargeRecordEntity, ReadTaskEntity readTaskEntity, ReadTaskEntity controlTask, PayOrderEntity payOrderEntity, MoneyAlarmEntity moneyAlarmEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Insert(readRecordEntity);
                if (chargeRecordEntity != null)
                    db.Insert(chargeRecordEntity);
                if (readTaskEntity != null)
                {
                    readTaskEntity.F_State = (int)TaskStateEnum.Finish;
                    readTaskEntity.F_ExecuteTime = DateTime.Now;
                    db.Update(readTaskEntity);
                }
                if (controlTask != null)
                    db.Insert(controlTask);
                if (payOrderEntity != null)
                    db.Update(payOrderEntity);
                if (moneyAlarmEntity != null)
                    db.Insert(moneyAlarmEntity);
                db.Commit();
            }
        }
    }
    public class ReadRecordViewRepository : RepositoryBase<ReadRecordViewModel>, IReadRecordViewRepository
    {

    }
    public class UnusualViewRepository : RepositoryBase<UnusualViewModel>, IUnusualViewRepository
    {

    }
}
