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
using NFine.Code;

namespace NFine.Application.SystemManage
{
    public class OrganizeApp
    {
        private IOrganizeRepository service = new OrganizeRepository();

        public List<OrganizeEntity> GetList(string companyId = "", int layers = 0)
        {
            if (string.IsNullOrEmpty(companyId) && layers == 0)
                return service.IQueryable().OrderBy(t => t.F_CreatorTime).ToList();
            if (string.IsNullOrEmpty(companyId) && layers > 0)
                return service.IQueryable(x => x.F_Layers == layers).OrderBy(t => t.F_CreatorTime).ToList();
            var rootNode = new List<OrganizeEntity>();
            var root = service.IQueryable().FirstOrDefault(x => x.F_Id == companyId);
            rootNode = GetModules(root, rootNode);
            rootNode.Add(root);
            return rootNode;
        }
        public List<OrganizeEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<OrganizeEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
            }
            expression = expression.And(t => t.F_ParentId == "0");
            return service.FindList(expression, pagination);
        }
        public List<OrganizeEntity> GetModules(OrganizeEntity item, List<OrganizeEntity> list)
        {
            var modules = service.IQueryable().Where(p => p.F_ParentId == item.F_Id).ToList();
            list = modules.Aggregate(list, (current, m) => GetModules(m, current));
            list.AddRange(modules);
            return list;
        }
        public OrganizeEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue, bool isAdmin = false)
        {
            if (service.IQueryable().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            var organize = GetForm(keyValue);
            if (organize.F_AllowDelete == false && !isAdmin)
                throw new Exception("删除失败！此机构不允许删除。");
            service.Delete(t => t.F_Id == keyValue);
        }
        public void SubmitForm(OrganizeEntity organizeEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                organizeEntity.Modify(keyValue);
                service.Update(organizeEntity);
            }
            else
            {
                organizeEntity.Create();
                service.Insert(organizeEntity);
            }
        }
        public void UpdateForm(OrganizeEntity organizeEntity)
        {
            service.Update(organizeEntity);
        }
        public void CheckEnable(string keyValue)
        {
            var organize = GetForm(keyValue);
            if (organize == null) throw new Exception("机构被系统删除,请联系管理员");
            while (organize.F_ParentId != "0")
            {
                var organize1 = organize;
                organize = service.FindEntity(x => x.F_Id == organize1.F_ParentId);
                if (organize == null)
                    throw new Exception("机构被系统删除,请联系管理员");
            }
            if (organize.F_DeleteMark == true || organize.F_EnabledMark == false)
                throw new Exception("机构被系统锁定,请联系管理员");
        }
        public List<string> GetSonIds(string keyValue)
        {
            return service.IQueryable(x => x.F_OwnerId == keyValue && x.F_DeleteMark != true).Select(x => x.F_Id).ToList();
        }
    }
}
