/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFine.Application.SystemManage
{
    public class PlatformApp
    {
        private readonly IPlatformRepository _service = new PlatformRepository();

        public List<PlatformEntity> GetList()
        {
            return _service.IQueryable().ToList();
        }
        public PlatformEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        public void SubmitForm(PlatformEntity platformEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                platformEntity.Modify(keyValue);
                _service.Update(platformEntity);
            }
            else
            {
                platformEntity.Create();
                _service.Insert(platformEntity);
            }
        }
    }
}
