/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using NFine.Domain.Enum;

namespace NFine.Application.SystemManage
{
    public class ModuleApp
    {
        private readonly IModuleRepository _service = new ModuleRepository();

        public List<ModuleEntity> GetList()
        {
            return _service.IQueryable().OrderBy(t => t.F_SortCode).ToList();
        }

        public List<ModuleEntity> GetList(CustomerTypeEnum customerType)
        {
            List<ModuleEntity> moduleList;
            switch (customerType)
            {
                case CustomerTypeEnum.System:
                    moduleList = _service.IQueryable(x => x.F_IsPublic != true || x.F_IsExpand == true).OrderBy(t => t.F_SortCode).ToList();
                    break;
                case CustomerTypeEnum.Platform:
                    moduleList = _service.IQueryable(x => x.F_IsExpand == true).OrderBy(t => t.F_SortCode).ToList();
                    break;
                case CustomerTypeEnum.Organization:
                    moduleList = _service.IQueryable(x => x.F_IsPublic == true).OrderBy(t => t.F_SortCode).ToList();
                    break;
                case CustomerTypeEnum.User:
                    moduleList = null;
                    break;
                default:
                    moduleList = _service.IQueryable().OrderBy(t => t.F_SortCode).ToList();
                    break;
            }
            return moduleList;
        }

        public ModuleEntity GetForm(string keyValue)
        {
            return _service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            if (_service.IQueryable().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                _service.Delete(t => t.F_Id == keyValue);
            }
        }
        public void SubmitForm(ModuleEntity moduleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                moduleEntity.Modify(keyValue);
                _service.Update(moduleEntity);
            }
            else
            {
                moduleEntity.Create();
                _service.Insert(moduleEntity);
            }
        }
    }
}
