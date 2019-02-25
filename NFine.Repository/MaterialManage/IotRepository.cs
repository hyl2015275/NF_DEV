/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Data;
using NFine.Domain.Entity.MaterialManage;
using NFine.Domain.IRepository.MaterialManage;

namespace NFine.Repository.MaterialManage
{
    public class IotRepository : RepositoryBase<IOTEntity>, IIotRepository
    {

    }
}
