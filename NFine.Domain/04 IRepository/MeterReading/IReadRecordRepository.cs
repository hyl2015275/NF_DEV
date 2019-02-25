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
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Domain.IRepository.MeterReading
{
    public interface IReadRecordRepository : IRepositoryBase<ReadRecordEntity>
    {
        void SubmitService(ReadRecordEntity readRecordEntity, ChargeRecordEntity chargeRecordEntity, ReadTaskEntity readTaskEntity, ReadTaskEntity controlTask, PayOrderEntity payOrderEntity, MoneyAlarmEntity moneyAlarmEntity);
    }
    public interface IReadRecordViewRepository : IRepositoryBase<ReadRecordViewModel>
    {

    }
    public interface IUnusualViewRepository : IRepositoryBase<UnusualViewModel>
    {

    }
}