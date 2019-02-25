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
using NFine.Domain.ViewModel.MeterReading;

namespace NFine.Domain.IRepository.MeterReading
{
    public interface IReadUnusualViewRepository : IRepositoryBase<ReadUnusualViewModel>
    {

    }
    public interface IReadUnusualRepository : IRepositoryBase<ReadUnusualEntity>
    {

    }
}
