﻿/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NFine.Code;
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Domain.IRepository.ArchiveManage
{
    public interface IChannelRepository : IRepositoryBase<ChannelEntity>
    {
        ChannelEntity GetFormByArchiveId(string archiveId);
    }
    public interface IChannelMeterRepository : IRepositoryBase<ChannelMeterEntity>
    {

    }
    public interface IChannelMeterViewRepository : IRepositoryBase<ChannelMeterViewModel>
    {

    }
}