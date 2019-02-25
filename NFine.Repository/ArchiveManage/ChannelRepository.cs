/*******************************************************************************
 * Copyright © 2018 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/

using System.Collections.Generic;
using NFine.Data;
using NFine.Domain.Entity.ArchiveManage;
using NFine.Domain.IRepository.ArchiveManage;
using NFine.Domain.ViewModel.ArchiveManage;

namespace NFine.Repository.ArchiveManage
{
    public class ChannelRepository : RepositoryBase<ChannelEntity>, IChannelRepository
    {
        public ChannelEntity GetFormByArchiveId(string archiveId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var channelMeter = db.FindEntity<ChannelMeterEntity>(x => x.F_ArchiveId == archiveId);
                return channelMeter != null ? db.FindEntity<ChannelEntity>(x => x.F_Id == channelMeter.F_ChannelId && x.F_DeleteMark != true) : null;
            }
        }
    }
    public class ChannelMeterRepository : RepositoryBase<ChannelMeterEntity>, IChannelMeterRepository
    {

    }
    public class ChannelMeterViewRepository : RepositoryBase<ChannelMeterViewModel>, IChannelMeterViewRepository
    {

    }
}