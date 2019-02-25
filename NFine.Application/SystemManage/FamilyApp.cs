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
    public class FamilyApp
    {
        private readonly IFamilyRepository service = new FamilyRepository();

        public List<FamilyEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
        public FamilyEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public FamilyEntity GetFormByUserName(string userName)
        {
            return service.FindEntity(x => x.F_Name == userName);
        }
        public FamilyEntity GetFormByOpenId(string openId)
        {
            return service.FindEntity(x => x.F_OpenId == openId);
        }

        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.F_Id == keyValue);
        }
        public void SubmitForm(FamilyEntity familyEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                service.Update(familyEntity);
            }
            else
            {
                service.Insert(familyEntity);
            }
        }
    }
}
