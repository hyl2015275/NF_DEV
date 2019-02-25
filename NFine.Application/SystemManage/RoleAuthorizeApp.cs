/*******************************************************************************
 * Copyright © 2017 淄博贝林电子有限公司 版权所有
 * Author: Lux
 * Description: Billion智能仪表管理系统
 * Website：http://www.billion-group.com
*********************************************************************************/
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Domain.ViewModel;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using NFine.Domain.Enum;

namespace NFine.Application.SystemManage
{
    public class RoleAuthorizeApp
    {
        private readonly IRoleAuthorizeRepository _service = new RoleAuthorizeRepository();
        private readonly ModuleApp _moduleApp = new ModuleApp();
        private readonly ModuleButtonApp _moduleButtonApp = new ModuleButtonApp();

        public List<RoleAuthorizeEntity> GetList(string objectId)
        {
            return _service.IQueryable(t => t.F_ObjectId == objectId).ToList();
        }
        public List<ModuleEntity> GetMenuList(string roleId)
        {
            var data = new List<ModuleEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = _moduleApp.GetList(CustomerTypeEnum.System);
            }
            else
            {
                var moduledata = OperatorProvider.Provider.GetCurrent().IsPlatform ? _moduleApp.GetList(CustomerTypeEnum.Platform) : _moduleApp.GetList(CustomerTypeEnum.Organization);
                var authorizedata = _service.IQueryable(t => t.F_ObjectId == roleId && t.F_ItemType == 1).ToList();
                data.AddRange(authorizedata.Select(item => moduledata.Find(t => t.F_Id == item.F_ItemId)).Where(moduleEntity => moduleEntity != null));
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }
        public List<ModuleButtonEntity> GetButtonList(string roleId)
        {
            var data = new List<ModuleButtonEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = _moduleButtonApp.GetList();
            }
            else
            {
                var buttondata = _moduleButtonApp.GetList();
                var authorizedata = _service.IQueryable(t => t.F_ObjectId == roleId && t.F_ItemType == 2).ToList();
                data.AddRange(authorizedata.Select(item => buttondata.Find(t => t.F_Id == item.F_ItemId)).Where(moduleButtonEntity => moduleButtonEntity != null));
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }
        public bool ActionValidate(string roleId, string moduleId, string action)
        {
            var authorizeurldata = new List<AuthorizeActionModel>();
            var cachedata = CacheFactory.Cache().GetCache<List<AuthorizeActionModel>>("authorizeurldata_" + roleId);
            if (cachedata == null)
            {
                var moduledata = _moduleApp.GetList();
                var buttondata = _moduleButtonApp.GetList();
                var authorizedata = _service.IQueryable(t => t.F_ObjectId == roleId).ToList();
                foreach (var item in authorizedata)
                {
                    if (item.F_ItemType == 1)
                    {
                        ModuleEntity moduleEntity = moduledata.Find(t => t.F_Id == item.F_ItemId);
                        authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleEntity.F_Id, F_UrlAddress = moduleEntity.F_UrlAddress });
                    }
                    else if (item.F_ItemType == 2)
                    {
                        ModuleButtonEntity moduleButtonEntity = buttondata.Find(t => t.F_Id == item.F_ItemId);
                        authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleButtonEntity.F_ModuleId, F_UrlAddress = moduleButtonEntity.F_UrlAddress });
                    }
                }
                CacheFactory.Cache().WriteCache(authorizeurldata, "authorizeurldata_" + roleId, DateTime.Now.AddMinutes(5));
            }
            else
            {
                authorizeurldata = cachedata;
            }
            authorizeurldata = authorizeurldata.FindAll(t => t.F_Id.Equals(moduleId));
            return (from item in authorizeurldata where !string.IsNullOrEmpty(item.F_UrlAddress) let url = item.F_UrlAddress.Split('?') where item.F_Id == moduleId && url[0] == action select item).Any();
        }
    }
}