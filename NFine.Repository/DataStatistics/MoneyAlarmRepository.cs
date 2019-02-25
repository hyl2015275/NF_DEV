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
using NFine.Domain.IRepository.DataStatistics;
using NFine.Domain.IRepository.MeterReading;
using NFine.Domain.ViewModel.DataStatistics;
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Repository.DataStatistics
{
    public class MoneyAlarmRepository : RepositoryBase<MoneyAlarmEntity>, IMoneyAlarmRepository
    {
      
    }
    public class MoneyAlarmViewRepository : RepositoryBase<MoneyAlarmViewModel>, IMoneyAlarmViewRepository
    {

    }
}
