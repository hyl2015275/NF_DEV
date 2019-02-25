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
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.Entity.MeterReading;
using NFine.Domain.Entity.PaymentManage;
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Domain.IRepository.MeterReading
{
    public interface IReadTaskRepository : IRepositoryBase<ReadTaskEntity>
    {
        void SubmitRechargeMoney(ReadTaskEntity readTaskEntity, PayOrderEntity payOrderEntity);
        void SubmitChangeMeter(ReadTaskEntity readTaskEntity, ChangeMeterEntity changeMeterEntity);
    }
    public interface IReadTaskViewRepository : IRepositoryBase<ReadTaskViewModel>
    {

    }
}
